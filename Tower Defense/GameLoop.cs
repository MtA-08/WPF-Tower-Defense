using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Tower_Defense;

namespace TowerDefense
{
	public class GameLoop : INotifyPropertyChanged
	{
		public double UpdateInterval = 1.0 / 120.0;
		private readonly Stopwatch clock = new();
		private bool isRunning;
		private double previousUpdateTime;
		private double _timeScale = 1.0;
		public double TimeScale { get => _timeScale; set { _timeScale = value; OnPropertyChanged(); } }
		public event Action<double> Frame;

		public void Start()
		{
			if (isRunning)
				return;

			clock.Restart();
			previousUpdateTime = 0;
			isRunning = true;

			CompositionTarget.Rendering += OnRendering;
		}

		public void Stop()
		{
			isRunning = false;

			CompositionTarget.Rendering -= OnRendering;

			clock.Stop();
		}

		private void OnRendering(object sender, EventArgs e)
		{

			if (!isRunning)
				return;

			double now = clock.Elapsed.TotalSeconds;
			double deltaTime = now - previousUpdateTime;


			previousUpdateTime = now;

			Frame?.Invoke(deltaTime);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}