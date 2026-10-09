using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using static Tower_Defense.Objects.BaseObject;

namespace Tower_Defense.Objects
{
	public class EnemyDefinition
	{
		public string TypeId { get; }
		public string SvgName { get; }
		public int HitboxRadius { get; }
		public double Speed { get; }
		public int HealthPoints { get; set; }
		public List<Position> WaypointPositions { get; }
		public int MoneyToDrop { get; }

		public EnemyDefinition() { }
		public EnemyDefinition(string typeId, string svgName, double speed, int hitboxRadius, int healthPoints,
							   List<Position> waypointPositions, int moneyToDrop)
		{ 
			TypeId = typeId;
			SvgName = svgName;
			HitboxRadius = hitboxRadius;
			Speed = speed;
			HealthPoints = healthPoints;
			WaypointPositions = waypointPositions;
			MoneyToDrop = moneyToDrop;
		}
	}

    public class Enemy : MovingObject
    {
		public EnemyDefinition Definition { get; }

		public int EnemyId { get; set; }
		public int HealthPoints => Definition.HealthPoints;
		public int RemainingHealthPoints {get; set; }
        public int MoneyToDrop => Definition.MoneyToDrop;
        public List<Position> WaypointPositions => Definition.WaypointPositions;
		public int NextWaypointIndex { get; set; } = 0;
		public Enemy() { }
		public Enemy(double posX, double posY, EnemyDefinition definition, List<Enemy> enemies)
			: base(posX, posY, definition.SvgName, definition.Speed, definition.HitboxRadius)
        {
			EnemyId = GenerateNewId(enemies);
			Definition = definition;
			RemainingHealthPoints = definition.HealthPoints;
		}

		public void Move(double deltaTime)
		{
			double movement = Speed * deltaTime;

			while (movement > 0 && NextWaypointIndex < WaypointPositions.Count)
			{
				var target = WaypointPositions[NextWaypointIndex];

				double dx = target.X - PosX;
				double dy = target.Y - PosY;

				double distance = Math.Sqrt(dx * dx + dy * dy);

				if (distance <= movement)
				{
					PosX = target.X;
					PosY = target.Y;

					movement -= distance;

					NextWaypointIndex++;
				}
				else
				{
					PosX += (dx / distance) * movement;
					PosY += (dy / distance) * movement;

					break;
				}
			}
		}

		public int GenerateNewId(List<Enemy> activeEnemies)
		{
			int newId = 1;
			if (activeEnemies.Any())
			{
				newId = activeEnemies.Max(id => id.EnemyId) + 1;
			}
			return newId;
		}
	}
}
