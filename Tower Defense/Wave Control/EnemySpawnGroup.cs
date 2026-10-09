namespace Tower_Defense.Objects
{
    public class EnemySpawnGroup
    {
		public EnemyDefinition Definition { get; }
		public int Count { get; }

		public EnemySpawnGroup(EnemyDefinition definition, int count)
		{
			Definition = definition;
			Count = count;
		}
	}
}
