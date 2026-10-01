using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Collections.Specialized.BitVector32;

namespace Tower_Defense
{
    /// <summary>
    /// Interaktionslogik für MainMenuWindow.xaml
    /// </summary>
    public partial class MainMenuWindow : Window
    {
        public MainMenuWindow()
        {
            InitializeComponent();
		}

		public MainWindow gameWindow;

        private void StartSingleplayerGame_Click(object sender, RoutedEventArgs e)
        {
			OpenGameWindow(GameMode.Singleplayer, true);
		}

        private void OpenMultiplayerOptions_Click(object sender, RoutedEventArgs e)
        {
			MulitplayerOptionsPopup.IsOpen = true;
		}

		public bool networkTestStarted;

		public async void HostButton_Click(object sender, RoutedEventArgs e)
		{
			if (networkTestStarted)
				return;

			networkTestStarted = true;

			try
			{
				OpenGameWindow(GameMode.Multiplayer, true);


				await gameWindow.controller.gameSession.HostWaitForClientConnectionAsync(50000);

				gameWindow.SetWaitingForPlayer(false, "Waiting for another Player ...");

				await ExchangeGameDataAsync();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Netzwerk");
			}
		}

		private async void JoinButton_Click(object sender, RoutedEventArgs e)
		{

			if (networkTestStarted)
				return;

			networkTestStarted = true;

			try
			{
				OpenGameWindow(GameMode.Multiplayer, false);

				await gameWindow.controller.gameSession.ClientConnectToHostAsync("127.0.0.1", 50000);

				await ExchangeGameDataAsync();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Netzwerk");
			}
		}

		private void OpenGameWindow(GameMode mode, bool isHost)
		{
			if (gameWindow == null)
			{
				gameWindow = new MainWindow();
			}

			gameWindow.controller.gameSession.GameMode = mode;
			gameWindow.controller.gameSession.IsHost = isHost;

			gameWindow.SetWaitingForPlayer(mode == GameMode.Multiplayer && isHost, "Waiting for antoher player ...");
			gameWindow.UpdateTitle();

			MulitplayerOptionsPopup.IsOpen = false;

			Application.Current.MainWindow = gameWindow;
			gameWindow.Show();
			Hide();
		}

		public async Task ReceiveGameMessagesAsync()
		{
			var gameSession = gameWindow.controller.gameSession;

			try
			{
				while (gameSession.IsConnected)
				{
					NetworkMessage message = await gameSession.ReceiveSnapshotDataAsync();

					if (message == null)
						return;

					if (message.Type == "Snapshot" && !gameSession.IsHost)
					{
						gameSession.StoreReceivedSnapshot(message.Snapshot);
					}
					else if (gameSession.IsHost &&
							 (message.Type == "SetPaused" ||
							  message.Type == "StartWave" ||
							  message.Type == "NextTimeScale" ||
							  message.Type == "BuildTower" ||
							  message.Type == "UpgradeTower"))
					{
						message.FromClient = true;

						gameSession.PendingCommands.Enqueue(message);
					}
				}
			}
			finally
			{
				gameSession.StopConnection();
			}
		}

		private Task ExchangeGameDataAsync()
		{
			return ReceiveGameMessagesAsync();
		}
	}
}
