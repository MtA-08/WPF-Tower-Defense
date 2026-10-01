using System.Collections.Generic;
using System.Windows.Media;
using TowerDefense;
using static Tower_Defense.Objects.BaseObject;

namespace Tower_Defense.Objects
{
	public class ObjectDefinitions
    {
		public List<Position> EnemyPathPoints { get; }
		public EnemyDefinition SlowWeakEnemyDefinition { get; }
		public EnemyDefinition SlowTankEnemyDefinition { get; }
		public EnemyDefinition FastHybridEnemyDefinition { get; }
		public EnemyDefinition HybridTankEnemyDefinition { get; }
		public EnemyDefinition LevelBossEnemyDefinition { get; }


		public ProjectileDefinition PebbleProjectileDefinition { get; }
		public ProjectileDefinition ArrowProjectileDefinition { get; }
		public ProjectileDefinition BulletProjectileDefinition { get; }
		public ProjectileDefinition SniperBulletProjectileDefinition { get; }
		public ProjectileDefinition TrapNetProjectileDefinition { get; }

		public TowerDefinition CavemanTowerDefinition { get; }
		public TowerDefinition ArcherTowerDefinition { get; }
		public TowerDefinition GunmanTowerDefinition { get; }
		public TowerDefinition SniperTowerDefinition { get; }
		public TowerDefinition HunterTowerDefinition { get; }

        public ObjectDefinitions()
        {
			/**************************************************************************************/
			EnemyPathPoints = new List<Position>
			{
				new Position(13.6, 244),
				new Position(260.8, 243.2),
				new Position(297.6, 257.6),
				new Position(312.8, 269.6),
				new Position(320, 279.2),
				new Position(324, 292.8),
				new Position(327.2, 307.2),
				new Position(328.8, 326.4),
				new Position(330.4, 504),
				new Position(349.6, 536),
				new Position(363.2, 549.6),
				new Position(374.4, 560),
				new Position(394.4, 568),
				new Position(413.6, 572),
				new Position(423.2, 573.6),
				new Position(619.2, 568),
				new Position(644.8, 560),
				new Position(663.2, 546.4),
				new Position(676, 523.2),
				new Position(685.6, 504.8),
				new Position(685.6, 478.4),
				new Position(685.6, 445.6),
				new Position(688.8, 417.6),
				new Position(692, 395.2),
				new Position(694.4, 377.6),
				new Position(701.6, 360),
				new Position(718.4, 342.4),
				new Position(740, 332.8),
				new Position(759.2, 330.4),
				new Position(769.6, 325.6),
				new Position(776, 325.6),
				new Position(963.2, 320),
				new Position(992, 327.2),
				new Position(1007.2, 332),
				new Position(1020, 344),
				new Position(1028.8, 362.4),
				new Position(1036, 380),
				new Position(1037.6, 397.6),
				new Position(1044, 509.6),
				new Position(1048.8, 529.6),
				new Position(1058.4, 544),
				new Position(1070.4, 559.2),
				new Position(1096, 572.8),
				new Position(1128, 580),
				new Position(1156, 577.6),
				new Position(1180, 575.2),
				new Position(1200, 574.4),
				new Position(1221.6, 571.2),
				new Position(1247.2, 573.6),
				new Position(1269.6, 572),
				new Position(1289.6, 570.4),
				new Position(1299.2, 569.6),
				new Position(1312, 561.6),
				new Position(1324, 552),
				new Position(1347.2, 535.2),
				new Position(1351.2, 520.8),
				new Position(1352, 500.8),
				new Position(1346.4, 296),
				new Position(1347.2, 270.4),
				new Position(1360, 243.2),
				new Position(1379.2, 234.4),
				new Position(1397.6, 226.4),
				new Position(1421.6, 220.8),
				new Position(1452.8, 220),
				new Position(1479.2, 220),
				new Position(1503.2, 219.2),
				new Position(1575.2, 221.6),
				new Position(1584, 220),
				new Position(1700, 220)
			};

			SlowWeakEnemyDefinition = new EnemyDefinition(
			typeId: "Slow Weak",
			svgName: "Lvl1Enemy.svg",
			hitboxRadius: 40,
			speed: 125,
			healthPoints: 300,
			waypointPositions: EnemyPathPoints,
			moneyToDrop: 10
			);

			SlowTankEnemyDefinition = new EnemyDefinition(
			typeId: "Slow Tank",
			svgName: "Lvl2Enemy.svg",
			hitboxRadius: 50,
			speed: 100,
			healthPoints: 950,
			waypointPositions: EnemyPathPoints,
			moneyToDrop: 40
			);

			FastHybridEnemyDefinition = new EnemyDefinition(
			typeId: "Fast Hybrid",
			svgName: "Lvl3Enemy.svg",
			hitboxRadius: 50,
			speed: 175,
			healthPoints: 800,
			waypointPositions: EnemyPathPoints,
			moneyToDrop: 50
			);

			HybridTankEnemyDefinition = new EnemyDefinition(
			typeId: "Hybrid Tank",
			svgName: "Lvl4Enemy.svg",
			hitboxRadius: 50,
			speed: 275,
			healthPoints: 1500,
			waypointPositions: EnemyPathPoints,
			moneyToDrop: 100
			);

			LevelBossEnemyDefinition = new EnemyDefinition(
			typeId: "Level Boss",
			svgName: "Boss.svg",
			hitboxRadius: 70,
			speed: 150,
			healthPoints: 6700,
			waypointPositions: EnemyPathPoints,
			moneyToDrop: 500
			);

			/**************************************************************************************/

			PebbleProjectileDefinition = new ProjectileDefinition(
			typeId: "Pebble",
			svgName: "Pebble.svg",
			speed: 500,
			hitboxRadius: 10,
			damage: 60,
			maxPenetrations: 1);

			ArrowProjectileDefinition = new ProjectileDefinition(
			typeId: "Arrow",
			svgName: "Arrow.svg",
			speed: 550,
			hitboxRadius: 15,
			damage: 75,
			maxPenetrations: 3);

			BulletProjectileDefinition = new ProjectileDefinition(
			typeId: "Bullet",
			svgName: "Bullet.svg",
			speed: 700,
			hitboxRadius: 15,
			damage: 150,
			maxPenetrations: 2);

			SniperBulletProjectileDefinition = new ProjectileDefinition(
			typeId: "Sniper Bullet",
			svgName: "SniperBullet.svg",
			speed: 2000,
			hitboxRadius: 15,
			damage: 900,
			maxPenetrations: 1);

			TrapNetProjectileDefinition = new ProjectileDefinition(
			typeId: "TrapNet",
			svgName: "TrapNet.svg",
			speed: 0,
			hitboxRadius: 50,
			damage: 10,
			maxPenetrations: 1);

			/**************************************************************************************/

			CavemanTowerDefinition = new TowerDefinition(
			typeId: "Caveman",
			svgName: "Caveman.svg",
			attackRangeRadius: 200,
			ticksPerAttack: 60,
			price: 60,
			projectile: PebbleProjectileDefinition,
			attackSpeedUpgradePrice: 25,
			damageUpgradePrice: 25
			);

			ArcherTowerDefinition = new TowerDefinition(
			typeId: "Archer",
			svgName: "Archer.svg",
			attackRangeRadius: 250,
			ticksPerAttack: 50,
			price: 180,
			projectile: ArrowProjectileDefinition,
			attackSpeedUpgradePrice: 70,
			damageUpgradePrice: 70
			);

			GunmanTowerDefinition = new TowerDefinition(
			typeId: "Gunman",
			svgName: "Gunman.svg",
			attackRangeRadius: 300,
			ticksPerAttack: 30,
			price: 600,
			projectile: BulletProjectileDefinition,
			attackSpeedUpgradePrice: 260,
			damageUpgradePrice: 260
			);

			SniperTowerDefinition = new TowerDefinition(
			typeId: "Sniper",
			svgName: "Sniper.svg",
			attackRangeRadius: 600,
			ticksPerAttack: 180,
			price: 1400,
			projectile: SniperBulletProjectileDefinition,
			attackSpeedUpgradePrice: 700,
			damageUpgradePrice: 700
			);

			HunterTowerDefinition = new TowerDefinition(
			typeId: "Hunter",
			svgName: "Hunter.svg",
			attackRangeRadius: 200000,
			ticksPerAttack: 240,
			price: 450,
			projectile: TrapNetProjectileDefinition,
			attackSpeedUpgradePrice: 140,
			damageUpgradePrice: 140
			);
		}
    }
}
