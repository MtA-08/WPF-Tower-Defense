using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tower_Defense.Objects;

namespace Tower_Defense
{
    public class WaveDefinition
    {
        public double SpawnIntervalTicks { get; }
        public List<EnemySpawnGroup> Groups { get; }

        public WaveDefinition(double spawnIntervalTicks, params EnemySpawnGroup[] groups) 
        { 
            SpawnIntervalTicks = spawnIntervalTicks;
            Groups = new List<EnemySpawnGroup>(groups);
        }
    }
}
