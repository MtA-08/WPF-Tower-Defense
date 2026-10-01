using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Tower_Defense
{
    public class GameSession : INotifyPropertyChanged
    {
        public bool IsHost { get; set; } = false;
        public GameMode GameMode { get; set; } = GameMode.Singleplayer;

		public TcpListener listener;
		public TcpClient connection;

		public StreamReader reader;
		public StreamWriter writer;

		public readonly SemaphoreSlim sendLock = new SemaphoreSlim(1, 1);

		public bool started;
		public bool closed;

		private bool _isConnected = false;
		public bool IsConnected { get => _isConnected; set { _isConnected = value; OnPropertyChanged(); } }
		public readonly ConcurrentQueue<NetworkMessage> PendingCommands	= new ConcurrentQueue<NetworkMessage>();


		public void StartConnection()
		{
			if (started || closed)
				return;

			started = true;
		}

		public void StopConnection()
		{
			closed = true;
			IsConnected = false;

			listener?.Stop();
			connection?.Close();
		}

		public void PrepareConnection()
		{
			if (closed)
			{
				connection.Close();
				return;
			}

			connection.NoDelay = true;

			NetworkStream stream = connection.GetStream();

			reader = new StreamReader(stream, Encoding.UTF8);

			writer = new StreamWriter(stream, new UTF8Encoding(false))
			{
				AutoFlush = true,
				NewLine = "\n"
			};

			IsConnected = true;
		}

		public async Task HostWaitForClientConnectionAsync(int port)
		{
			StartConnection();

			try
			{
				listener = new TcpListener(IPAddress.Loopback, port);
				listener.Start();

				connection = await listener.AcceptTcpClientAsync();

				PrepareConnection();
			}
			catch
			{
				StopConnection();
				throw;
			}
			finally
			{
				listener?.Stop();
			}
		}

		public async Task ClientConnectToHostAsync(string ip, int port)
		{
			StartConnection();

			try
			{
				connection = new TcpClient();

				await connection.ConnectAsync(IPAddress.Parse(ip), port);

				PrepareConnection();
			}
			catch
			{
				StopConnection();
				throw;
			}
		}

		public async Task SendSnapshotDataAsync(NetworkMessage message)
		{
			if (message == null)
				return;

			string json = JsonConvert.SerializeObject(message, Formatting.None);

			await sendLock.WaitAsync();

			try
			{
				if (!IsConnected)
					throw new InvalidOperationException("Keine Verbindung vorhanden.");

				await writer.WriteLineAsync(json);
			}
			catch
			{
				StopConnection();
				throw;
			}
			finally
			{
				sendLock.Release();
			}
		}

		public async Task<NetworkMessage> ReceiveSnapshotDataAsync()
		{
			if (!IsConnected)
				throw new InvalidOperationException("Keine Verbindung vorhanden.");

			try
			{
				string json = await reader.ReadLineAsync();

				if (json == null)
				{
					StopConnection();
					return null;
				}

				NetworkMessage message = JsonConvert.DeserializeObject<NetworkMessage>(json);

				if (message == null)
					throw new InvalidDataException("Ungültige Nachricht.");

				return message;
			}
			catch
			{
				StopConnection();
				throw;
			}
		}

		public GameSnapshot pendingSnapshot;

		public void StoreReceivedSnapshot(GameSnapshot snapshot)
		{
			if (snapshot == null || snapshot.Towers == null)
				throw new InvalidDataException("Ungültiger Snapshot.");

			Interlocked.Exchange(ref pendingSnapshot, snapshot);
		}

		public GameSnapshot TakeReceivedSnapshot()
		{
			return Interlocked.Exchange(ref pendingSnapshot, null);
		}

		private bool _isPaused;

		public bool IsPaused
		{
			get => _isPaused;
			set
			{
				if (_isPaused == value)
					return;

				_isPaused = value;
				OnPropertyChanged();
			}
		}

		public async Task RequestGameCommandAsync(NetworkMessage message)
		{
			if (GameMode == GameMode.Multiplayer && !IsConnected)
				throw new InvalidOperationException("Keine Verbindung vorhanden.");

			if (GameMode == GameMode.Singleplayer || IsHost)
			{
				PendingCommands.Enqueue(message);
			}
			else
			{
				await SendSnapshotDataAsync(message);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public class NetworkMessage
	{
		public string Type { get; set; }
		public string Text { get; set; }
		public bool IsPaused { get; set; }

		public GameSnapshot Snapshot { get; set; }

		public TowerBuildRequest TowerBuild { get; set; }
		public TowerUpgradeRequest TowerUpgrade { get; set; }

		[JsonIgnore]
		public bool FromClient { get; set; }
	}

	public class TowerBuildRequest
	{
		public double PosX { get; set; }
		public double PosY { get; set; }
		public string DefinitionTypeId { get; set; }
	}

	public class TowerUpgradeRequest
	{
		public int TowerId { get; set; }
		public string UpgradeType { get; set; }
	}

	public enum GameMode { Singleplayer, Multiplayer }
}
