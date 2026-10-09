using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Tower_Defense.Objects
{
	public class ProjectileDefinition
	{
		public string TypeId { get; }
		public string SvgName { get; }
		public double Speed { get; }
		public int HitboxRadius { get; }
		public int Damage { get; }
		public int MaxPenetrations { get; }

		public ProjectileDefinition(string typeId, string svgName, double speed, 
									int hitboxRadius, int damage, int maxPenetrations)
		{
			TypeId = typeId;
			SvgName = svgName;
			Speed = speed;
			HitboxRadius = hitboxRadius;
			Damage = damage;
			MaxPenetrations = maxPenetrations;
		}
	}

	public class Projectile : MovingObject
	{
		public ProjectileDefinition Definition { get; }
		public int ProjectileId { get; set; }
		public int Damage { get; }
		public int MaxPenetrations => Definition.MaxPenetrations;
		public int RemainingPenetrations { get; private set; }

		private double directionX;
		private double directionY;

		private readonly HashSet<Enemy> hitEnemies = new HashSet<Enemy>();

		public Projectile() { }
		public Projectile(double posX, double posY, ProjectileDefinition definition, double damageMultiplier, List<Projectile> projectiles)
			: base(posX, posY, definition.SvgName, definition.Speed, definition.HitboxRadius)
		{
			ProjectileId = GenerateNewId(projectiles);
			Definition = definition;
			RemainingPenetrations = definition.MaxPenetrations;
			Damage = (int)Math.Round(definition.Damage * damageMultiplier, MidpointRounding.AwayFromZero);
		}

		public bool InitializeDirection(double targetX, double targetY)
		{
			double dx = targetX - PosX;
			double dy = targetY - PosY;

			double distance = Math.Sqrt(dx * dx + dy * dy);

			if (distance == 0)
				return false;

			directionX = dx / distance;
			directionY = dy / distance;

			return true;
		}

		public void FindClosestPositionOnPathAndSetProjectilePosition(Tower originTower, List<Position> pathPoints)
		{
			var closestPositionX = 0.0;
			var closestPositionY = 0.0;
			var shortestDistanceSquared = double.PositiveInfinity;

			foreach (var position in pathPoints)
			{
				double dx = position.X - originTower.PosX;
				double dy = position.Y - originTower.PosY;

				double distanceSquared = dx * dx + dy * dy;

				if (distanceSquared < shortestDistanceSquared)
				{
					shortestDistanceSquared = distanceSquared;

					closestPositionX = position.X;
					closestPositionY = position.Y;
				}
			}

			PosX = closestPositionX;
			PosY = closestPositionY;
		}

		public void Move(double deltaTime)
		{

			double movement = Speed * deltaTime;

			PosX += directionX * movement;
			PosY += directionY * movement;
		}

		public void FollowEnemy(double deltaTime, double enemyPosX, double enemyPosY)
		{
			if (deltaTime <= 0 || RemainingPenetrations <= 0)
				return;

			double dx = enemyPosX - PosX;
			double dy = enemyPosY - PosY;

			double distance = Math.Sqrt(dx * dx + dy * dy);

			if (distance == 0)
				return;

			directionX = dx / distance;
			directionY = dy / distance;

			double movement = Math.Min(Speed * deltaTime, distance);

			PosX += directionX * movement;
			PosY += directionY * movement;
		}

		public bool HasHitEnemy(Enemy enemy)
		{
			return hitEnemies.Contains(enemy);
		}

		public bool TryHit(Enemy enemy)
		{
			if (enemy == null || enemy.HealthPoints <= 0 || RemainingPenetrations <= 0)
			{
				return false;
			}

			if (!hitEnemies.Add(enemy))
				return false;

			enemy.RemainingHealthPoints = Math.Max(0, enemy.RemainingHealthPoints - Damage);

			RemainingPenetrations--;

			return true;
		}

		public bool IsProjectileOutOfBoundsOrHasNoPenetrations()
		{
			if (PosX <= -20 || PosX >= 1620 || PosY <= -20 || PosY >= 920 || RemainingPenetrations <= 0)
				return true;
			else
				return false;
		}

		public int GenerateNewId(List<Projectile> activeProjectiles)
		{
			int newId = 1;
			if (activeProjectiles.Any())
			{
				newId = activeProjectiles.Max(id => id.ProjectileId) + 1;
			}
			return newId;
		}
	}
}