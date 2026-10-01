using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using TowerDefense;

namespace Tower_Defense.Objects
{
	public class TowerDefinition
	{
		public string TypeId { get; }
		public string SvgName { get; }
		public int AttackRangeRadius { get; }
		public int PlacementRadius { get; }
		public double TicksPerAttack { get; }
		public int Price { get; }
		public ProjectileDefinition Projectile { get; }
		public int AttackSpeedUpgradePrice { get; }
		public int DamageUpgradePrice { get; }

		public TowerDefinition(string typeId, string svgName, 
							   int attackRangeRadius, double ticksPerAttack, 
							   int price, ProjectileDefinition projectile,
							   int attackSpeedUpgradePrice, int damageUpgradePrice )
		{ 
			TypeId = typeId;
			SvgName = svgName;
			AttackRangeRadius = attackRangeRadius;
			PlacementRadius = 35;
			TicksPerAttack = ticksPerAttack;
			Price = price;
			Projectile = projectile;
			AttackSpeedUpgradePrice = attackSpeedUpgradePrice;
			DamageUpgradePrice = damageUpgradePrice;
		}

		public void DeductTowerPriceFromPlayerBalance(int price, PlayerData playerData)
		{
			playerData.Money -= price;
		}

		public void DeductUpgradePriceFromPlayerBalance(int upgradePrice, PlayerData playerData)
		{
			playerData.Money -= upgradePrice;
		}
	}

    public class Tower : StaticObject, INotifyPropertyChanged
    {
		public TowerDefinition Definition { get; }
		public int TowerId { get; set; }
		public double TicksPerAttack => Definition.TicksPerAttack - AttackSpeedMultiplier*30;
		public double TicksSinceLastAttack {get; set;}
		private double _attackSpeedMultiplier = 0;
		public double AttackSpeedMultiplier { get => _attackSpeedMultiplier; set { _attackSpeedMultiplier = value; OnPropertyChanged(); } }
		private double _damageMultiplier = 1.0;
		public double DamageMultiplier {get => _damageMultiplier; set { _damageMultiplier = value; OnPropertyChanged();} }

		public Tower(double posX, double posY, TowerDefinition definition, List<Tower> towers) 
			: base(posX, posY, definition.SvgName)
		{
			TowerId = GenerateNewId(towers);
			Definition = definition;
			TicksSinceLastAttack = definition.TicksPerAttack;
		}

		public int GenerateNewId(List<Tower> activeTowers)
		{
			int newId = 1;
			if (activeTowers.Any())
			{
				newId = activeTowers.Max(id => id.TowerId) + 1;
			}
			return newId;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
