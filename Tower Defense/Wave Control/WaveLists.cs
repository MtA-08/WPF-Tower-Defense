using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tower_Defense.Objects;
using TowerDefense;

namespace Tower_Defense
{
	public class WaveLists
	{
		public List<WaveDefinition> Waves { get; }

		public WaveLists(ObjectDefinitions definitions)
		{
			var weak = definitions.SlowWeakEnemyDefinition;
			var tank = definitions.SlowTankEnemyDefinition;
			var fast = definitions.FastHybridEnemyDefinition;
			var hybrid = definitions.HybridTankEnemyDefinition;
			var boss = definitions.LevelBossEnemyDefinition;

			Waves = new List<WaveDefinition>
			{
                // Wave 1: Opening.
                new WaveDefinition(180, Group(weak, 8)),

                // Wave 2: More basic enemies.
                new WaveDefinition(174, Group(weak, 10)),

                // Wave 3: Introduce one Slow Tank.
                new WaveDefinition(180, Group(weak, 8), Group(tank, 1), Group(weak, 2)),

                // Wave 4: Practice against tanks.
                new WaveDefinition(168, Group(weak, 8), Group(tank, 2), Group(weak, 2)),

                // Wave 5: First mixed checkpoint.
                new WaveDefinition(156, Group(weak, 10), Group(tank, 3)),

                // Wave 6: Introduce one Fast Hybrid.
                new WaveDefinition(168, Group(weak, 6), Group(tank, 3), Group(fast, 1), Group(weak, 4)),

                // Wave 7: Two fast enemies.
                new WaveDefinition(156, Group(weak, 6), Group(tank, 4), Group(fast, 2), Group(weak, 2)),

                // Wave 8: Build coverage.
                new WaveDefinition(144, Group(weak, 6), Group(tank, 5), Group(fast, 3), Group(weak, 2)),

                // Wave 9: Mixed pressure.
                new WaveDefinition(138, Group(weak, 8), Group(tank, 6), Group(fast, 4)),

                // Wave 10: First chapter checkpoint.
                new WaveDefinition(132, Group(weak, 8), Group(tank, 8), Group(fast, 4)),

                // Wave 11: Recovery.
                new WaveDefinition(156, Group(weak, 10), Group(tank, 6), Group(fast, 4)),

                // Wave 12: Introduce one Hybrid Tank.
                new WaveDefinition(168,Group(weak, 6), Group(tank, 5), Group(fast, 3), Group(hybrid, 1), Group(weak, 2)),

                // Wave 13: Two Hybrid Tanks.
                new WaveDefinition(150, Group(weak, 6), Group(tank, 6), Group(fast, 4), Group(hybrid, 2)),

                // Wave 14: Combined speed and health.
                new WaveDefinition(144, Group(weak, 8), Group(tank, 7), Group(fast, 5), Group(hybrid, 3)),

                // Wave 15: Midgame checkpoint.
                new WaveDefinition(138, Group(weak, 6), Group(tank, 8), Group(fast, 6), Group(hybrid, 4)),

                // Wave 16: Recovery.
                new WaveDefinition(156, Group(weak, 10), Group(tank, 8), Group(fast, 5), Group(hybrid, 3)),

                // Wave 17: Prepare stronger damage.
                new WaveDefinition(138, Group(weak, 8), Group(tank, 10), Group(fast, 6), Group(hybrid, 4)),

                // Wave 18: Faster groups.
                new WaveDefinition(132, Group(weak, 8), Group(tank, 10), Group(fast, 8), Group(hybrid, 5)),

                // Wave 19: Boss preparation.
                new WaveDefinition(126, Group(weak, 6), Group(tank, 12), Group(fast, 10), Group(hybrid, 6)),

                // Wave 20: First boss, at the end of a lighter wave.
                new WaveDefinition(180, Group(weak, 6), Group(tank, 6), Group(fast, 4), Group(boss, 1)),

                // Wave 21: Return to normal enemies.
                new WaveDefinition(150, Group(weak, 12), Group(tank, 10), Group(fast, 8), Group(hybrid, 4)),

                // Wave 22: Sustained pressure.
                new WaveDefinition(138, Group(weak, 8), Group(tank, 12), Group(fast, 10), Group(hybrid, 6)),

                // Wave 23: More tanks.
                new WaveDefinition(132, Group(weak, 8), Group(tank, 14), Group(fast, 12), Group(hybrid, 6)),

                // Wave 24: Dense mixed groups.
                new WaveDefinition(120, Group(weak, 6), Group(tank, 14), Group(fast, 14), Group(hybrid, 8)),

                // Wave 25: One boss with modest following escorts.
                new WaveDefinition(150, Group(weak, 8), Group(tank, 8), Group(boss, 1), Group(weak, 10), Group(fast, 6), Group(hybrid, 4)),

                // Wave 26: Stronger fast group.
                new WaveDefinition(126, Group(weak, 8), Group(tank, 14), Group(fast, 12), Group(hybrid, 10)),

                // Wave 27: Late-game preparation.
                new WaveDefinition(120, Group(weak, 6), Group(tank, 16), Group(fast, 14), Group(hybrid, 12)),

                // Wave 28: Sustained fast pressure.
                new WaveDefinition(114, Group(weak, 8), Group(tank, 16), Group(fast, 16), Group(hybrid, 14)),

                // Wave 29: Second boss checkpoint preparation.
                new WaveDefinition(108, Group(weak, 6), Group(tank, 18), Group(fast, 18), Group(hybrid, 16)),

                // Wave 30: Two bosses separated by 25.2 seconds.
                new WaveDefinition(144, Group(boss, 1), Group(weak, 6), Group(tank, 8), Group(fast, 4),
					                    Group(hybrid, 2), Group(boss, 1), Group(weak, 6), Group(tank, 6),
					                    Group(fast, 4), Group(hybrid, 2)),

                // Wave 31: Recovery.
                new WaveDefinition(132, Group(weak, 12), Group(tank, 16), Group(fast, 16), Group(hybrid, 12)),

                // Wave 32: Final chapter begins.
                new WaveDefinition(114, Group(weak, 10), Group(tank, 18), Group(fast, 18), Group(hybrid, 14)),

                // Wave 33: Mixed endurance.
                new WaveDefinition(108, Group(weak, 8), Group(tank, 20), Group(fast, 20), Group(hybrid, 16)),

                // Wave 34: Faster stream.
                new WaveDefinition(102, Group(weak, 8), Group(tank, 20), Group(fast, 24), Group(hybrid, 18)),

                // Wave 35: Two well-separated bosses with escorts.
                new WaveDefinition(132, Group(boss, 1), Group(weak, 10), Group(tank, 10), Group(fast, 8),
					                    Group(hybrid, 5), Group(boss, 1), Group(weak, 10), Group(tank, 8),
					                    Group(fast, 8), Group(hybrid, 5)),

                // Wave 36: Return to sustained pressure.
                new WaveDefinition(108, Group(weak, 10), Group(tank, 22), Group(fast, 22), Group(hybrid, 18)),

                // Wave 37: Stronger mixed groups.
                new WaveDefinition(102, Group(weak, 8), Group(tank, 24), Group(fast, 24), Group(hybrid, 20)),

                // Wave 38: Final upgrades.
                new WaveDefinition(96, Group(weak, 10), Group(tank, 24), Group(fast, 28), Group(hybrid, 22)),

                // Wave 39: Fastest spawn rate.
                new WaveDefinition(90, Group(weak, 8), Group(tank, 26), Group(fast, 30), Group(hybrid, 24)),

                // Wave 40: Three bosses, each separated by 18.9 seconds.
                new WaveDefinition(108, Group(boss, 1), Group(weak, 4), Group(tank, 6), Group(fast, 6),
					                    Group(hybrid, 4), Group(boss, 1), Group(weak, 4), Group(tank, 6),
					                    Group(fast, 6), Group(hybrid, 4), Group(boss, 1), Group(weak, 4),
					                    Group(tank, 10), Group(fast, 12), Group(hybrid, 10)),
			};
		}

		// Shorthand for the existing EnemySpawnGroup constructor.
		private static EnemySpawnGroup Group(EnemyDefinition definition, int count)
		{
			return new EnemySpawnGroup(definition, count);
		}
	}
}
