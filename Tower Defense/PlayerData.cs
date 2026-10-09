using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tower_Defense
{
    public class PlayerData : INotifyPropertyChanged
    {
        public PlayerData()
        { 
        }

        private double _money = 110;
        public double Money { get => _money; set { _money = value; OnPropertyChanged(); } }

        private double _spentMoney = 0;
        public double SpentMoney { get => _spentMoney; set { _spentMoney = value; OnPropertyChanged(); } }
        private int _remainingLives = 10;
        public int RemainingLives { get => _remainingLives; set { _remainingLives = value; OnPropertyChanged(); } }

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
