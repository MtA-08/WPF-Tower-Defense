using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Tower_Defense;
using Tower_Defense.Objects;

namespace TowerDefense
{
	public class GameController : INotifyPropertyChanged
	{
		public  GameVisual gameVisual;
		public  GameLoop gameLoop;
		public GameSession gameSession;
		public ObjectDefinitions definitions;
		public WaveLists waveLists;
		public PlayerData playerData;
		public PlayerData clientPlayerData;

		public List<Enemy> ActiveEnemies { get; set; } = new List<Enemy>();
		public Dictionary<Enemy, DrawingVisual> enemyVisuals = new Dictionary<Enemy, DrawingVisual>();

		public List<Projectile> ActiveProjectiles { get; set; } = new List<Projectile>();
		public Dictionary<Projectile, DrawingVisual> projectileVisuals = new Dictionary<Projectile, DrawingVisual>();

		public List<Tower> ActiveTowers { get; set; } = new List<Tower>();
		public Dictionary<Tower, DrawingVisual> towerVisuals = new Dictionary<Tower, DrawingVisual>();

		private readonly Dictionary<int, TowerSnapshot> snapshotTowers = new Dictionary<int, TowerSnapshot>();
		private readonly Dictionary<int, DrawingVisual> snapshotTowerVisuals = new Dictionary<int, DrawingVisual>();
		private readonly Dictionary<int, EnemySnapshot> snapshotEnemies = new Dictionary<int, EnemySnapshot>();
		private readonly Dictionary<int, DrawingVisual> snapshotEnemyVisuals = new Dictionary<int, DrawingVisual>();
		private readonly Dictionary<int, ProjectileSnapshot> snapshotProjectiles = new Dictionary<int, ProjectileSnapshot>();
		private readonly Dictionary<int, DrawingVisual> snapshotProjectileVisuals = new Dictionary<int, DrawingVisual>();

		private const double SnapshotIntervalSeconds = 0.1;

		private readonly ClientVisualInterpolator clientVisualInterpolator = new ClientVisualInterpolator(SnapshotIntervalSeconds);

		private bool _gameWon = false;
		public bool GameWon { get => _gameWon; set { _gameWon = value; OnPropertyChanged(); } }
		private bool _gameLost = false;
		public bool GameLost { get => _gameLost; set { _gameLost = value; OnPropertyChanged(); } }

		public GameController() { }

		public GameController(GameVisual host)
		{
			definitions = new ObjectDefinitions();
			playerData = new PlayerData();
            clientPlayerData = new PlayerData
            {
                Money = playerData.Money,
                RemainingLives = playerData.RemainingLives
            };

            gameVisual = host;
			gameSession = new GameSession();
			gameLoop = new GameLoop();

			

			gameLoop.Frame += GameLoop;
		}

		/**************************************************************************************/
		public Tower CreateTower(double posX, double posY, TowerDefinition towerDefinition)
		{
			var newTower = new Tower(posX, posY, towerDefinition, ActiveTowers);
			var visual = gameVisual.AddTower(
			newTower.PosX,
			newTower.PosY,
			newTower.Definition.AttackRangeRadius,
			newTower.SvgName
			);

			ActiveTowers.Add(newTower);
			towerVisuals.Add(newTower, visual);

			return newTower;
		}

		public void UpdateTowers(double deltaTime)
		{
			foreach (Tower currentTower in ActiveTowers)
			{
				if (!(currentTower.TicksSinceLastAttack >= currentTower.TicksPerAttack))
					currentTower.TicksSinceLastAttack ++;
			}
		}
		public void RemoveTower(Tower towerToRemove)
		{
			if (!towerVisuals.TryGetValue(towerToRemove, out DrawingVisual visual))
				return;

			gameVisual.RemoveVisual(visual);
			towerVisuals.Remove(towerToRemove);
			ActiveTowers.Remove(towerToRemove);
		}

		public void RemoveFinishedTowers()
		{
			for (int i = ActiveTowers.Count - 1; i >= 0; i--)
			{
				var currentTower = ActiveTowers[i];
			}
		}

		public void TryBuyTower(double posX, double posY, TowerDefinition towerDefinition)
		{
			if (gameSession.GameMode == GameMode.Multiplayer && !gameSession.IsHost)
			{
				return;
			}

			if (playerData.Money < towerDefinition.Price)
			{
				MessageBox.Show($"You dont have enough money to purchase this tower.", "Insufficent funds!", MessageBoxButton.OK, MessageBoxImage.Warning);

			}
			else if (IsTowerOnRoad(posX, posY, towerDefinition.PlacementRadius))
			{
				MessageBox.Show($"Towers can't be placed on the road!", "Invalid position for tower", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else if (IsTowerOnTower(posX, posY, towerDefinition.PlacementRadius))
			{
				MessageBox.Show($"Towers can't be placed on other towers!", "Invalid position for tower", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else
			{
				towerDefinition.DeductTowerPriceFromPlayerBalance(towerDefinition.Price, playerData);
				CreateTower(posX, posY, towerDefinition);
			}
		}

		private void AddOrUpdateSnapshotTower(TowerSnapshot tower)
		{
			if (!snapshotTowerVisuals.TryGetValue(
				tower.TowerId, out DrawingVisual visual))
			{
				visual = gameVisual.AddTower(
					tower.PosX,
					tower.PosY,
					tower.Definition.AttackRangeRadius,
					tower.SvgName);

				snapshotTowerVisuals.Add(tower.TowerId, visual);
			}

			snapshotTowers[tower.TowerId] = tower;

			visual.Offset = new Vector(tower.PosX, tower.PosY);
		}

		private void RemoveMissingSnapshotTowers(HashSet<int> receivedTowerIds)
		{
			var knownTowerIds = new List<int>(snapshotTowerVisuals.Keys);

			foreach (int towerId in knownTowerIds)
			{
				if (receivedTowerIds.Contains(towerId))
					continue;

				gameVisual.RemoveVisual(snapshotTowerVisuals[towerId]);

				snapshotTowerVisuals.Remove(towerId);
				snapshotTowers.Remove(towerId);
			}
		}

		public bool TryGetSnapshotTower(int towerId, out TowerSnapshot tower)
		{
			return snapshotTowers.TryGetValue(towerId, out tower);
		}

		/**************************************************************************************/
		/**************************************************************************************/
		public Projectile CreateProjectile(Tower originTower, ProjectileDefinition projectileDefinition, Enemy target)
		{ 
			Projectile newProjectile = new Projectile(originTower.PosX, originTower.PosY, projectileDefinition, originTower.DamageMultiplier, ActiveProjectiles);

			
			newProjectile.InitializeDirection(target.PosX, target.PosY);
			

			DrawingVisual visual = gameVisual.AddProjectile(
			newProjectile.PosX,
			newProjectile.PosY,
			newProjectile.HitboxRadius,
			newProjectile.SvgName
			);
			
			ActiveProjectiles.Add(newProjectile);
			projectileVisuals.Add(newProjectile, visual);

			return newProjectile;
		}

		private void UpdateProjectiles(double deltaTime)
		{
			foreach (Projectile currentProjectile in ActiveProjectiles)
			{
				currentProjectile.Move(deltaTime);
			}
		}
		public void RemoveProjectile(Projectile projectileToRemove)
		{
            if (!projectileVisuals.TryGetValue(projectileToRemove, out DrawingVisual visual))
                return;

			gameVisual.RemoveVisual(visual);
			projectileVisuals.Remove(projectileToRemove);
			ActiveProjectiles.Remove(projectileToRemove);
		}

		public void RemoveFinishedProjectiles()
		{
			for (int i = ActiveProjectiles.Count - 1; i >= 0; i--)
			{
				Projectile currentProjectile = ActiveProjectiles[i];

				if (currentProjectile.IsProjectileOutOfBoundsOrHasNoPenetrations())
				{
					RemoveProjectile(currentProjectile);
				}
			}
		}

		private void AddOrUpdateSnapshotProjectile(ProjectileSnapshot projectile)
		{
			if (!snapshotProjectileVisuals.TryGetValue(
				projectile.ProjectileId, out DrawingVisual visual))
			{
				visual = gameVisual.AddProjectile(
					projectile.PosX,
					projectile.PosY,
					projectile.Definition.HitboxRadius,
					projectile.SvgName);

				snapshotProjectileVisuals.Add(projectile.ProjectileId, visual);
			}

			snapshotProjectiles[projectile.ProjectileId] = projectile;

			clientVisualInterpolator.SetTarget(visual, new Vector(projectile.PosX, projectile.PosY), gameSession.IsPaused);
		}

		private void RemoveMissingSnapshotProjectiles(HashSet<int> receivedProjectileIds)
		{
			var knownProjectileIds = new List<int>(snapshotProjectileVisuals.Keys);

			foreach (int projectileId in knownProjectileIds)
			{
				if (receivedProjectileIds.Contains(projectileId))
					continue;

				clientVisualInterpolator.Remove(snapshotProjectileVisuals[projectileId]);

				gameVisual.RemoveVisual(snapshotProjectileVisuals[projectileId]);

				snapshotProjectileVisuals.Remove(projectileId);
				snapshotProjectiles.Remove(projectileId);
			}
		}
		/**************************************************************************************/
		/**************************************************************************************/
		public Enemy CreateEnemy(double posX, double posY, EnemyDefinition enemyDefinition)
		{
			Enemy newEnemy = new Enemy(posX, posY, enemyDefinition, ActiveEnemies);
			DrawingVisual visual = gameVisual.AddEnemy(
			newEnemy.PosX,
			newEnemy.PosY,
			newEnemy.HitboxRadius,
			newEnemy.SvgName
			);

			ActiveEnemies.Add(newEnemy);
			enemyVisuals.Add(newEnemy, visual);

			return newEnemy;
		}

		private void UpdateEnemies(double deltaTime)
		{
			foreach (Enemy currentEnemy in ActiveEnemies)
			{
				currentEnemy.Move(deltaTime);
			}
		}

		public void RemoveEnemy(Enemy enemyToRemove)
		{

            if (!enemyVisuals.TryGetValue(enemyToRemove, out DrawingVisual visual))
                return;

			gameVisual.RemoveVisual(visual);
			enemyVisuals.Remove(enemyToRemove);
			ActiveEnemies.Remove(enemyToRemove);
		}

		public void RemoveFinishedEnemies()
		{
			for (int i = ActiveEnemies.Count - 1; i >= 0; i--)
			{
				Enemy currentEnemy = ActiveEnemies[i];

				if (currentEnemy.NextWaypointIndex >= currentEnemy.WaypointPositions.Count)
				{
					playerData.RemainingLives--;
					RemoveEnemy(currentEnemy);
					if (playerData.RemainingLives <= 0)
					{
						GameLost = true;
					}
				}
				else if (currentEnemy.RemainingHealthPoints <= 0)
				{
					playerData.Money += currentEnemy.MoneyToDrop / 2;

					if (gameSession.GameMode == GameMode.Multiplayer && gameSession.IsHost)
					{
						clientPlayerData.Money += currentEnemy.MoneyToDrop / 2;
					}
					RemoveEnemy(currentEnemy);
				}				
			}
		}

		private void AddOrUpdateSnapshotEnemy(EnemySnapshot enemy)
		{
			if (!snapshotEnemyVisuals.TryGetValue(
				enemy.EnemyId, out DrawingVisual visual))
			{
				visual = gameVisual.AddEnemy(
					enemy.PosX,
					enemy.PosY,
					enemy.Definition.HitboxRadius,
					enemy.SvgName);

				snapshotEnemyVisuals.Add(enemy.EnemyId, visual);
			}

			snapshotEnemies[enemy.EnemyId] = enemy;

			clientVisualInterpolator.SetTarget(visual, new Vector(enemy.PosX, enemy.PosY),gameSession.IsPaused);
		}

		private void RemoveMissingSnapshotEnemies(HashSet<int> receivedEnemyIds)
		{
			var knownEnemyIds = new List<int>(snapshotEnemyVisuals.Keys);

			foreach (int enemyId in knownEnemyIds)
			{
				if (receivedEnemyIds.Contains(enemyId))
					continue;

				clientVisualInterpolator.Remove(snapshotEnemyVisuals[enemyId]);

				gameVisual.RemoveVisual(snapshotEnemyVisuals[enemyId]);

				snapshotEnemyVisuals.Remove(enemyId);
				snapshotEnemies.Remove(enemyId);
			}
		}
		/**************************************************************************************/


		public void Start()
		{
			gameLoop.Start();
		}

		public void Stop()
		{
			gameLoop.Stop();
		}

		

		private double DistanceSquaredToPoint(double x, double y, double startX, double startY, double endX, double endY)
		{
			var segmentX = endX - startX;
			var segmentY = endY - startY;

			var lengthSquared =
				segmentX * segmentX + segmentY * segmentY;

			double t = 0;

			if (lengthSquared > 0)
			{
				t = ((x - startX) * segmentX +
					 (y - startY) * segmentY) / lengthSquared;

				t = Math.Max(0, Math.Min(1, t));
			}

			var closestX = startX + t * segmentX;
			var closestY = startY + t * segmentY;

			var dx = x - closestX;
			var dy = y - closestY;

			return dx * dx + dy * dy;
		}

		public bool IsTowerOnRoad(double posX, double posY, double radius)
		{
			var roadHalfWidth = 25;

			var clearance = roadHalfWidth + radius;
			var clearanceSqr = clearance * clearance;

			var path = definitions.EnemyPathPoints;

            for (int i = 0; i < path.Count; i++)
            {
				var start = path[i];
				var end = path[i];

				if ( i+1 != 69)
					end = path[i+1];

				var distanceSquared = DistanceSquaredToPoint(
				posX, posY,
				start.X, start.Y,
				end.X, end.Y);

				if (distanceSquared <= clearanceSqr)
					return true;
			}

			return false;
        }

		public bool IsTowerOnTower(double posX, double posY, double placementRadius)
		{ 
			var clearance = placementRadius + placementRadius;
			var clearanceSqr = clearance * clearance;

			foreach (var tower in ActiveTowers)
			{
				var distanceSqr = DistanceSquaredToPoint(posX, posY, tower.PosX, tower.PosY, tower.PosX, tower.PosY);

				if (distanceSqr <= clearanceSqr)
					return true;
			}

			return false;
		}

		public void CheckProjectileCollision()
		{

			foreach (Enemy enemy in ActiveEnemies)
			{
				foreach (Projectile shot in ActiveProjectiles)
						{
					double dx = shot.PosX - enemy.PosX;
					double dy = shot.PosY - enemy.PosY;

					double combinedRadius = shot.HitboxRadius + enemy.HitboxRadius;

					if (dx * dx + dy * dy <= combinedRadius * combinedRadius)
					{
						shot.TryHit(enemy);
					}	
				}
			}
		}

		public void ShootEnemiesInsideAnyTowerRange()
		{

			foreach (Tower tower in ActiveTowers)
			{
				foreach (Enemy enemy in ActiveEnemies)
				{
					double dx = tower.PosX - enemy.PosX;
					double dy = tower.PosY - enemy.PosY;

					double combinedRadius = tower.Definition.AttackRangeRadius + enemy.HitboxRadius;

					if (dx * dx + dy * dy <= combinedRadius * combinedRadius)
					{
						if (tower.TicksSinceLastAttack >= tower.TicksPerAttack)
						{
							CreateProjectile(tower, tower.Definition.Projectile, enemy);
							tower.TicksSinceLastAttack = 0;
						}
					}
				}
			}
		}

		private int _currentWaveIndex = 0;
		public int CurrentWaveIndex { get => _currentWaveIndex; set { _currentWaveIndex = value; OnPropertyChanged(); } }
		public int currentGroupIndex = 0;
		public int spawnedInCurrentGroup = 0;
		public double ticksUntilNextSpawn = 0;
		public bool startNextWave = false;
		public void SpawnWave()
		{
			if (startNextWave)
			{
				waveLists = new WaveLists(definitions);
				var currentWave = waveLists.Waves[CurrentWaveIndex];

				while (currentGroupIndex < currentWave.Groups.Count && spawnedInCurrentGroup >= currentWave.Groups[currentGroupIndex].Count)
				{
					currentGroupIndex++;
					spawnedInCurrentGroup = 0;
				}

				if (currentGroupIndex >= currentWave.Groups.Count)
				{
					if (ActiveEnemies.Count > 0)
						return;

					CurrentWaveIndex++;
					currentGroupIndex = 0;
					spawnedInCurrentGroup = 0;
					ticksUntilNextSpawn = 0;
					gameLoop.TimeScale = 1;
					startNextWave = false;

					if (CurrentWaveIndex >= waveLists.Waves.Count)
					{
						GameWon = true;
					}

					return;
				}

				if (ticksUntilNextSpawn > 0)
					ticksUntilNextSpawn -- ;

				if (ticksUntilNextSpawn > 0)
					return;

				var currentGroup = currentWave.Groups[currentGroupIndex];

				CreateEnemy(-60, 244, currentGroup.Definition);

				spawnedInCurrentGroup++;
				ticksUntilNextSpawn = currentWave.SpawnIntervalTicks;

				if (CurrentWaveIndex >= waveLists.Waves.Count)
				{
					GameWon = true;
				}
			}
		}

		public double snapshotTimer;
		public bool snapshotSendInProgress;

		private async Task SendCurrentSnapshotAsync()
		{
			snapshotSendInProgress = true;

			try
			{
				await gameSession.SendSnapshotDataAsync(new NetworkMessage
				{
					Type = "Snapshot",
					Snapshot = CreateSnapshot()
				});
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				gameSession.StopConnection();
			}
			finally
			{
				snapshotSendInProgress = false;
			}
		}

		public GameSnapshot CreateSnapshot()
		{
			var snapshot = new GameSnapshot();

			foreach (var tower in ActiveTowers)
			{
				snapshot.Towers.Add(new TowerSnapshot
				{
					PosX = tower.PosX,
					PosY = tower.PosY,
					SvgName = tower.SvgName,
					Definition = tower.Definition,
					TowerId = tower.TowerId,
					TicksPerAttack = tower.TicksPerAttack,
					TicksSinceLastAttack = tower.TicksSinceLastAttack,
					AttackSpeedMultiplier = tower.AttackSpeedMultiplier,
					DamageMultiplier = tower.DamageMultiplier
				});
			}

			foreach (var enemy in ActiveEnemies)
			{
				snapshot.Enemies.Add(new EnemySnapshot
				{
					PosX = enemy.PosX,
					PosY = enemy.PosY,
					SvgName = enemy.SvgName,
					Definition = enemy.Definition,
					EnemyId = enemy.EnemyId,
					HealthPoints = enemy.HealthPoints,
					RemainingHealthPoints = enemy.RemainingHealthPoints,
					MoneyToDrop = enemy.MoneyToDrop,
					WaypointPositions = enemy.WaypointPositions,
					NextWaypointIndex = enemy.NextWaypointIndex
				});
			}

			foreach (var projectile in ActiveProjectiles)
			{
				snapshot.Projectiles.Add(new ProjectileSnapshot
				{
					PosX = projectile.PosX,
					PosY = projectile.PosY,
					SvgName = projectile.SvgName,
					Definition = projectile.Definition,
					ProjectileId = projectile.ProjectileId,
					MaxPenetrations = projectile.MaxPenetrations,
					RemainingPenetrations = projectile.RemainingPenetrations
				});
			}

			snapshot.IsPaused = gameSession.IsPaused;
			snapshot.TimeScale = gameLoop.TimeScale;
			snapshot.IsWaveRunning = startNextWave;
			snapshot.CurrentWaveIndex = CurrentWaveIndex;
			snapshot.RemainingLives = playerData.RemainingLives;
			snapshot.HostMoney = playerData.Money;
			snapshot.ClientMoney = clientPlayerData.Money;

			return snapshot;
		}

		public void TrySendSnapshot(double deltaTime)
		{
			if (gameSession.GameMode != GameMode.Multiplayer ||
				!gameSession.IsHost ||
				!gameSession.IsConnected)
			{
				return;
			}

			snapshotTimer += deltaTime;

			if (snapshotTimer < SnapshotIntervalSeconds || snapshotSendInProgress)
				return;

			snapshotTimer %= SnapshotIntervalSeconds;

			_ = SendCurrentSnapshotAsync();
		}

		public void ProcessGameCommands()
		{
			while (gameSession.PendingCommands.TryDequeue(out NetworkMessage message))
			{
				if (message.Type == "SetPaused")
				{
					gameSession.IsPaused = message.IsPaused;

					Debug.WriteLine($"Host: Pause geändert auf {gameSession.IsPaused}");
				}
				else if (message.Type == "StartWave")
				{
					if (!gameSession.IsPaused && !startNextWave)
					{
						startNextWave = true;
					}
				}
				else if (message.Type == "NextTimeScale")
				{
					if (!gameSession.IsPaused && startNextWave)
					{
						gameLoop.TimeScale = gameLoop.TimeScale >= 4 ? 1 : gameLoop.TimeScale + 1;
					}
				}
				else if (message.Type == "BuildTower")
				{
					if (!GameWon && !GameLost)
						ProcessTowerBuild(message);
				}
				else if (message.Type == "UpgradeTower")
				{
					if (!GameWon && !GameLost)
						ProcessTowerUpgrade(message);
				}
				else
				{
					Debug.WriteLine("Unbekannter Spielbefehl: " + message.Type);
				}
			}
		}

		private TowerDefinition FindTowerDefinition(string typeId)
		{
			if (typeId == definitions.CavemanTowerDefinition.TypeId)
				return definitions.CavemanTowerDefinition;

			if (typeId == definitions.ArcherTowerDefinition.TypeId)
				return definitions.ArcherTowerDefinition;

			if (typeId == definitions.GunmanTowerDefinition.TypeId)
				return definitions.GunmanTowerDefinition;

			if (typeId == definitions.SniperTowerDefinition.TypeId)
				return definitions.SniperTowerDefinition;

			return null;
		}

		private void ProcessTowerBuild(NetworkMessage message)
		{
			if (message.TowerBuild == null)
				return;

			TowerDefinition definition =
				FindTowerDefinition(
					message.TowerBuild.DefinitionTypeId);

			if (definition == null)
				return;

			PlayerData buyer =
				message.FromClient
				? clientPlayerData
				: playerData;

			double posX = message.TowerBuild.PosX;
			double posY = message.TowerBuild.PosY;

			if (buyer.Money < definition.Price)
				return;

			if (IsTowerOnRoad(
				posX,
				posY,
				definition.PlacementRadius))
			{
				return;
			}

			if (IsTowerOnTower(
				posX,
				posY,
				definition.PlacementRadius))
			{
				return;
			}

			definition.DeductTowerPriceFromPlayerBalance(
				definition.Price,
				buyer);

			CreateTower(posX, posY, definition);
		}

		private void ProcessTowerUpgrade(NetworkMessage message)
		{
			if (message.TowerUpgrade == null)
				return;

			Tower towerToUpgrade = null;

			foreach (Tower tower in ActiveTowers)
			{
				if (tower.TowerId ==
					message.TowerUpgrade.TowerId)
				{
					towerToUpgrade = tower;
					break;
				}
			}

			if (towerToUpgrade == null)
				return;

			PlayerData buyer = message.FromClient ? clientPlayerData : playerData;

			if (message.TowerUpgrade.UpgradeType == "Damage")
			{
				if (buyer.Money <
					towerToUpgrade.Definition.DamageUpgradePrice)
				{
					return;
				}

				if (towerToUpgrade.DamageMultiplier >= 2.5)
					return;

				towerToUpgrade.DamageMultiplier += 0.5;

				towerToUpgrade.Definition
					.DeductTowerPriceFromPlayerBalance(towerToUpgrade.Definition.DamageUpgradePrice, buyer);
			}
			else if (message.TowerUpgrade.UpgradeType == "AttackSpeed")
			{
				if (buyer.Money < towerToUpgrade.Definition.AttackSpeedUpgradePrice)
				{
					return;
				}

				if (towerToUpgrade.AttackSpeedMultiplier >= 0.15)
					return;

				towerToUpgrade.AttackSpeedMultiplier += 0.05;

				towerToUpgrade.Definition.DeductTowerPriceFromPlayerBalance(towerToUpgrade.Definition.AttackSpeedUpgradePrice,buyer);
			}
		}

		public bool TryGetSnapshotTowerAt(double posX, double posY, out TowerSnapshot selectedTower)
		{
			foreach (TowerSnapshot tower in snapshotTowers.Values)
			{
				double radius = tower.Definition.PlacementRadius;

				bool insideX = posX >= tower.PosX - radius && posX <= tower.PosX + radius;

				bool insideY = posY >= tower.PosY - radius && posY <= tower.PosY + radius;

				if (insideX && insideY)
				{
					selectedTower = tower;
					return true;
				}
			}

			selectedTower = null;
			return false;
		}

		private void UpdateObjectVisualsHost()
		{
			foreach (var enemy in ActiveEnemies)
			{
				DrawingVisual visual = enemyVisuals[enemy];

				visual.Offset = new Vector(enemy.PosX, enemy.PosY);
			}

			foreach (var projectile in ActiveProjectiles)
			{
				DrawingVisual visual = projectileVisuals[projectile];

				visual.Offset = new Vector(projectile.PosX, projectile.PosY);
			}

			foreach (var tower in ActiveTowers)
			{
				DrawingVisual visual = towerVisuals[tower];

				visual.Offset = new Vector(tower.PosX, tower.PosY);
			}
		}

		private void UpdateObjectVisualsClient(GameSnapshot snapshot)
		{
			var receivedTowerIds = new HashSet<int>();

			foreach (TowerSnapshot tower in snapshot.Towers)
			{
				receivedTowerIds.Add(tower.TowerId);

				AddOrUpdateSnapshotTower(tower);
			}

			RemoveMissingSnapshotTowers(receivedTowerIds);

			var receivedEnemyIds = new HashSet<int>();

			foreach (EnemySnapshot enemy in snapshot.Enemies)
			{
				receivedEnemyIds.Add(enemy.EnemyId);

				AddOrUpdateSnapshotEnemy(enemy);
			}

			RemoveMissingSnapshotEnemies(receivedEnemyIds);

			var receivedProjectileIds = new HashSet<int>();

			foreach (ProjectileSnapshot projectile in snapshot.Projectiles)
			{
				receivedProjectileIds.Add(projectile.ProjectileId);

				AddOrUpdateSnapshotProjectile(projectile);
			}

			RemoveMissingSnapshotProjectiles(receivedProjectileIds);
		}
		private void UpdateLogic(double deltaTime)
		{
			SpawnWave();
			
			UpdateTowers(deltaTime);
			UpdateProjectiles(deltaTime);
			UpdateEnemies(deltaTime);

			CheckProjectileCollision();

			RemoveFinishedEnemies();
			RemoveFinishedProjectiles();

			ShootEnemiesInsideAnyTowerRange();
		}

		public double accumulator = 0.0;
		
		public void GameLoop(double deltaTime)
		{
			var mainWindow = Application.Current.MainWindow as MainWindow;

			if (mainWindow == null)
				return;

			if (gameSession.GameMode == GameMode.Singleplayer || gameSession.IsHost)
			{
				bool ready = gameSession.GameMode == GameMode.Singleplayer || gameSession.IsConnected;

				if (ready)
				{
					ProcessGameCommands();
				}

				if (ready && !gameSession.IsPaused)
				{
					accumulator += deltaTime * gameLoop.TimeScale;

					while (accumulator >= gameLoop.UpdateInterval)
					{
						UpdateLogic(gameLoop.UpdateInterval);
						accumulator -= gameLoop.UpdateInterval;
					}
				}

				TrySendSnapshot(deltaTime);

				UpdateObjectVisualsHost();
			}
			else
			{
				if (gameSession.IsConnected && !gameSession.IsPaused)
				{
					clientVisualInterpolator.Update(deltaTime);
				}

				GameSnapshot snapshot = gameSession.TakeReceivedSnapshot();

				if (snapshot != null)
				{
					gameSession.IsPaused = snapshot.IsPaused;
					gameLoop.TimeScale = snapshot.TimeScale;
					startNextWave = snapshot.IsWaveRunning;
					CurrentWaveIndex = snapshot.CurrentWaveIndex;
					playerData.RemainingLives = snapshot.RemainingLives;
					playerData.Money =snapshot.ClientMoney;

					UpdateObjectVisualsClient(snapshot);
					mainWindow.RefreshSelectedSnapshotTower();
				}
			}
			
			mainWindow.UpdateConditionText();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}