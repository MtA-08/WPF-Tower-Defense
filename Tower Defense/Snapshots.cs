using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Tower_Defense.Objects;
using static Tower_Defense.Objects.BaseObject;

namespace Tower_Defense
{
	public class GameSnapshot
	{
		public bool IsPaused { get; set; }
		public double TimeScale { get; set; } = 1.0;
		public bool IsWaveRunning { get; set; }
		public int CurrentWaveIndex { get; set; }
		public int RemainingLives { get; set; }
		public double HostMoney { get; set; }
		public double ClientMoney { get; set; }

		public List<TowerSnapshot> Towers { get; set; }
			= new List<TowerSnapshot>();
		public List<EnemySnapshot> Enemies { get; set; }
			= new List<EnemySnapshot>();
		public List<ProjectileSnapshot> Projectiles{ get; set; }
			= new List<ProjectileSnapshot>();
	}
	public class TowerSnapshot : INotifyPropertyChanged
	{
		public double PosX { get; set; }
		public double PosY { get; set; }
		public string SvgName { get; set; }
		public TowerDefinition Definition { get; set; }
		public int TowerId { get; set; }
		public double TicksPerAttack { get; set; }
		public double TicksSinceLastAttack { get; set; }
		public double AttackSpeedMultiplier { get; set; }
		public double DamageMultiplier { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public class EnemySnapshot : INotifyPropertyChanged
	{
		public double PosX { get; set; }
		public double PosY { get; set; }
		public string SvgName { get; set; }
		public double Speed { get; set; }
		public int HitboxRadius { get; set; }
		public EnemyDefinition Definition { get; set; }
		public int EnemyId { get; set; }
		public int HealthPoints { get; set; }
		public int RemainingHealthPoints { get; set; }
		public int MoneyToDrop { get; set; }
		public List<Position> WaypointPositions { get; set; }
		public int NextWaypointIndex { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	public class ProjectileSnapshot : INotifyPropertyChanged
	{
		public double PosX { get; set; }
		public double PosY { get; set; }
		public string SvgName { get; set; }
		public double Speed { get; set; }
		public int HitboxRadius { get; set; }
		public ProjectileDefinition Definition { get; set; }
		public int ProjectileId { get; set; }
		public int MaxPenetrations { get; set; }
		public int RemainingPenetrations { get; set; }

		public double directionX { get; set; }
		public double directionY { get; set; }

		public HashSet<Enemy> hitEnemies { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
 