using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tower_Defense.Objects;
using TowerDefense;
using static Tower_Defense.Objects.BaseObject;

namespace Tower_Defense
{
    public partial class MainWindow : Window
    {
		public MainWindow()
		{
			InitializeComponent();

			controller = new GameController(GameSurface);

			CaveManTowerCard.DataContext = controller.definitions.CavemanTowerDefinition;
			ArcherTowerCard.DataContext = controller.definitions.ArcherTowerDefinition;
			// HunterTowerCard.DataContext = definitions.HunterTowerDefinition;
			GunmanTowerCard.DataContext = controller.definitions.GunmanTowerDefinition;
			SniperTowerCard.DataContext = controller.definitions.SniperTowerDefinition;

			PlayerInformationContainer.DataContext = controller.playerData;
			WaveCounter.DataContext = controller;
			StartWaveToggleSpeedButtonContent.DataContext = controller.gameLoop;
			GameEndConditionPopupBackgroundImg.DataContext = controller.GameWon;

			Loaded += (sender, e) => controller.Start();

			Closed += (sender, e) =>
			{
				controller.Stop();
				controller.gameSession.StopConnection();
			};



			// File.WriteAllText("click_positions.txt", string.Empty);
		}

		public void UpdateTitle()
		{
			if (controller.gameSession.GameMode == GameMode.Multiplayer)
			{
				if (controller.gameSession.IsHost)
				{
					Title = "Tower Defense - Multiplayer (Player A)";
				}
				else
				{
					Title = "Tower Defense - Multiplayer (Player B)";
				}
			}
			else
				Title = "Tower Defense";
		}

		public GameController controller;

		private void GameArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (IsWaitingForPlayer)
				return;

			Point position = e.GetPosition(GameSurface);

			if (IsMultiplayerClient)
			{
				if (controller.TryGetSnapshotTowerAt(position.X, position.Y, out TowerSnapshot snapshotTower))
				{
					clickedSnapshotTower = snapshotTower;
					clickedTower = null;

					TowerModificationMenuData.DataContext = clickedSnapshotTower;

					TowerModificationMenu.IsOpen = true;
				}

				return;
			}

			clickedTower = CheckClickPositionForTowerAndReturnClickedTower(position.X, position.Y);

			if (clickIsOnTower)
			{
				clickedSnapshotTower = null;

				clickedTowerDeletePrice = (int)Math.Round(clickedTower.Definition.Price * 0.8, MidpointRounding.AwayFromZero);

				TowerModificationMenuData.DataContext = clickedTower;

				TowerDeletePrice.DataContext = clickedTowerDeletePrice;

				TowerModificationMenu.IsOpen = true;
			}
		}

		public TowerSnapshot clickedSnapshotTower;
		private bool IsMultiplayerClient
		{
			get
			{
				return controller.gameSession.GameMode ==
						   GameMode.Multiplayer
					   && !controller.gameSession.IsHost;
			}
		}
		public bool clickIsOnTower = false;

		public Tower CheckClickPositionForTowerAndReturnClickedTower(double posX, double posY)
		{
            foreach (var tower in controller.ActiveTowers)
            {
				var towerAreaMaxX = tower.PosX + tower.Definition.PlacementRadius;
				var towerAreaMinX = tower.PosX - tower.Definition.PlacementRadius;
				var towerAreaMaxY = tower.PosY + tower.Definition.PlacementRadius;
				var towerAreaMinY = tower.PosY - tower.Definition.PlacementRadius;

				if ((posX <= towerAreaMaxX && posX >= towerAreaMinX) && (posY <= towerAreaMaxY && posY >= towerAreaMinY))
				{ 
					clickIsOnTower = true;
					return tower;
				}
			}
			clickIsOnTower = false;
			return null;
        }

		public Tower clickedTower;
		public int clickedTowerDeletePrice = 0;

		private async void UpgradeTowerDamage_Click(object sender, RoutedEventArgs e)
		{
			if (IsMultiplayerClient)
			{
				if (clickedSnapshotTower == null)
					return;

				await RequestGameActionAsync(type: "UpgradeTower", towerUpgrade: new TowerUpgradeRequest
				{
					TowerId =clickedSnapshotTower.TowerId,
					UpgradeType = "Damage"
				});

				return;
			}

			if (controller.playerData.Money >=
				clickedTower.Definition.DamageUpgradePrice)
			{
				if (clickedTower.DamageMultiplier < 2.5)
				{
					clickedTower.DamageMultiplier += 0.5;
					clickedTower.Definition.DeductTowerPriceFromPlayerBalance(clickedTower.Definition.DamageUpgradePrice, controller.playerData);
				}
			}
		}

		private async void UpgradeTowerAttackSpeed_Click(object sender, RoutedEventArgs e)
		{
			if (IsMultiplayerClient)
			{
				if (clickedSnapshotTower == null)
					return;

				await RequestGameActionAsync(type: "UpgradeTower", towerUpgrade: new TowerUpgradeRequest
				{
					TowerId =clickedSnapshotTower.TowerId,
					UpgradeType = "AttackSpeed"
				});

				return;
			}

			if (controller.playerData.Money >= clickedTower.Definition.AttackSpeedUpgradePrice)
			{
				if (clickedTower.AttackSpeedMultiplier < 0.15)
				{
					clickedTower.AttackSpeedMultiplier += 0.05;
					clickedTower.Definition.DeductTowerPriceFromPlayerBalance(clickedTower.Definition.AttackSpeedUpgradePrice, controller.playerData);
				}
			}
		}

		private void SellTower_Click(object sender, RoutedEventArgs e)
		{
			controller.playerData.Money += clickedTowerDeletePrice;
			controller.RemoveTower(clickedTower);
			TowerModificationMenu.IsOpen = false;
		}

		private void CloseTowerModificationMenu_Click(object sender, RoutedEventArgs e)
		{ 
			TowerModificationMenu.IsOpen = false;
		}

		private async void StartWaveToggleSpeed_Click(object sender, RoutedEventArgs e)
		{
			if (IsWaitingForPlayer || controller.gameSession.IsPaused || controller.GameWon || controller.GameLost)
				return;

			if (!controller.startNextWave)
			{
				await RequestGameActionAsync("StartWave");
			}
			else
			{
				await RequestGameActionAsync("NextTimeScale");
			}
		}

		private void CaveManTowerCard_MouseMove(object sender, MouseEventArgs e)
		{
			if (IsWaitingForPlayer)
				return;

			if (e.LeftButton == MouseButtonState.Pressed)
			{
				
				if (sender is Grid sendingGrid && sendingGrid.DataContext is TowerDefinition towerData)
				{
					DataObject data = new DataObject();

					data.SetData(typeof(TowerDefinition), towerData);
					data.SetData(typeof(Grid), sendingGrid);

					DragDrop.DoDragDrop(CaveManTowerCard, data, DragDropEffects.Move);
				}
			}
		}

		private async void Window_Drop(object sender, DragEventArgs e)
		{
			if (IsWaitingForPlayer)
				return;

			Point position = e.GetPosition(GameSurface);

			if (!(e.Data.GetData(typeof(TowerDefinition)) is TowerDefinition chosenDefinition))
			{
				return;
			}

			bool isMultiplayerClient = controller.gameSession.GameMode == GameMode.Multiplayer && !controller.gameSession.IsHost;

			if (isMultiplayerClient)
			{
				await RequestGameActionAsync(type: "BuildTower", towerBuild: new TowerBuildRequest
				{
					PosX = position.X,
					PosY = position.Y,
					DefinitionTypeId =
					chosenDefinition.TypeId
				});

				return;
			}

			controller.TryBuyTower(position.X, position.Y, chosenDefinition);
		}

		private async void ToggleSettingsMenu_Click(object sender, RoutedEventArgs e)
		{
			if (IsWaitingForPlayer)
				return;

			bool pauseState = !controller.gameSession.IsPaused;

			await RequestGameActionAsync(type: "SetPaused", isPaused: pauseState);
		}

		private void RestartRound_Click(object sender, RoutedEventArgs e)
        {
			if(controller.ActiveTowers.Count > 0)
            {
				var count = controller.ActiveTowers.Count;
				for (var i = count - 1; i >= 0; i--)
				{
					controller.RemoveTower(controller.ActiveTowers[i]);
				}
			}

			if (controller.ActiveProjectiles.Count > 0)
			{
				var count = controller.ActiveProjectiles.Count;
				for (var i = count - 1; i >= 0; i--)
				{
					controller.RemoveProjectile(controller.ActiveProjectiles[i]);
				}
			}

			if (controller.ActiveEnemies.Count > 0)
			{
				var count = controller.ActiveEnemies.Count;
				for (var i = count - 1; i >= 0; i--)
				{
					controller.RemoveEnemy(controller.ActiveEnemies[i]);
				}
			}

			controller.gameSession.IsPaused = false;
			controller.CurrentWaveIndex = 0;
			controller.currentGroupIndex = 0;
			controller.spawnedInCurrentGroup = 0;
			controller.ticksUntilNextSpawn = 0;
			controller.gameLoop.TimeScale = 1;
			controller.startNextWave = false;

			controller.playerData.Money = 110;
			controller.playerData.RemainingLives = 10;
			controller.playerData.SpentMoney = 0;

			controller.GameWon = false;
			controller.GameLost = false;

			TowerModificationMenu.IsOpen = false;
			SettingsMenu.IsOpen = false;
			GameEndConditionPopUp.IsOpen = false;
		}

		public MainMenuWindow mainMenu;

		private void GoToMainMenu_Click(object sender, EventArgs e)
		{
			if (mainMenu == null)
			{ 
				mainMenu = new MainMenuWindow();
			}

			if (controller.ActiveTowers.Count > 0)
			{
				var count = controller.ActiveTowers.Count;
				for (var i = count - 1; i >= 0; i--)
				{
					controller.RemoveTower(controller.ActiveTowers[i]);
				}
			}

			if (controller.ActiveProjectiles.Count > 0)
			{
				var count = controller.ActiveProjectiles.Count;
				for (var i = count - 1; i >= 0; i--)
				{
					controller.RemoveProjectile(controller.ActiveProjectiles[i]);
				}
			}

			if (controller.ActiveEnemies.Count > 0)
			{
				var count = controller.ActiveEnemies.Count;
				for (var i = count - 1; i >= 0; i--)
				{
					controller.RemoveEnemy(controller.ActiveEnemies[i]);
				}
			}

			controller.accumulator = 0;
			controller.gameSession.IsPaused = false;
			controller.CurrentWaveIndex = 0;
			controller.currentGroupIndex = 0;
			controller.spawnedInCurrentGroup = 0;
			controller.ticksUntilNextSpawn = 0;
			controller.gameLoop.TimeScale = 1;
			controller.startNextWave = false;

			controller.playerData.Money = 100;
			controller.playerData.RemainingLives = 10;
			controller.playerData.SpentMoney = 0;

			controller.GameWon = false;
			controller.GameLost = false;

			TowerModificationMenu.IsOpen = false;
			SettingsMenu.IsOpen = false;
			GameEndConditionPopUp.IsOpen = false;

			Hide();
			mainMenu.Show();
			
		}

		public void UpdateConditionText()
		{
			SettingsMenu.IsOpen = controller.gameSession.IsPaused;

			if (controller.GameLost)
			{
				GameEndConditionText.Text = "YOU LOST";
				GameEndConditionPopUp.IsOpen = true;
				GameEndConditionPopupBackgroundImg.Source = new BitmapImage(new Uri("/images/gameLostBackground.png", UriKind.Relative));
				MoneySpentThisRound.Text = controller.playerData.SpentMoney.ToString();
				controller.gameLoop.Stop();
			}
			if (controller.GameWon)
			{
				GameEndConditionText.Text = "VICTORY"; 
				GameEndConditionPopUp.IsOpen = true;
				GameEndConditionPopupBackgroundImg.Source = new BitmapImage(new Uri("/images/gameWonBackground.png", UriKind.Relative));
				MoneySpentThisRound.Text = controller.playerData.SpentMoney.ToString();
				controller.gameLoop.Stop();
			}

			if (!controller.GameLost && !controller.GameWon)
			{
				GameEndConditionText.Text = "";
			}
		}

		public bool IsWaitingForPlayer { get; private set; }

		public void SetWaitingForPlayer(bool waiting, string text)
		{
			IsWaitingForPlayer = waiting;

			WaitingForPlayerPopup.IsOpen = waiting;
			WaitingForPlayerPopupText.Text = text;

			GameSurface.IsEnabled = !waiting;
		}
		public async Task RequestGameActionAsync(string type, bool isPaused = false, TowerBuildRequest towerBuild = null, TowerUpgradeRequest towerUpgrade = null)
		{
			try
			{
				NetworkMessage message = new NetworkMessage
				{
					Type = type,
					IsPaused = isPaused,
					TowerBuild = towerBuild,
					TowerUpgrade = towerUpgrade
				};

				await controller.gameSession.RequestGameCommandAsync(message);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Spielsteuerung");
			}
		}

		public void RefreshSelectedSnapshotTower()
		{
			if (clickedSnapshotTower == null)
				return;

			if (controller.TryGetSnapshotTower(clickedSnapshotTower.TowerId, out TowerSnapshot latestTower))
			{
				clickedSnapshotTower = latestTower;

				if (TowerModificationMenu.IsOpen)
				{
					TowerModificationMenuData.DataContext = clickedSnapshotTower;
				}
			}
			else
			{
				clickedSnapshotTower = null;
				TowerModificationMenuData.DataContext = null;
				TowerModificationMenu.IsOpen = false;
			}
		}

		// ############################# DEV CONTROLS #####################################
		private void SpawnSlowWeakEnemy_Click(object sender, RoutedEventArgs e)
		{
			controller.CreateEnemy(-60, 244, controller.definitions.SlowWeakEnemyDefinition);
		}
		private void SpawnSlowTankEnemy_Click(object sender, RoutedEventArgs e)
		{
			controller.CreateEnemy(-60, 244, controller.definitions.SlowTankEnemyDefinition);
		}
		private void SpawnFastHybridEnemy_Click(object sender, RoutedEventArgs e)
		{
			controller.CreateEnemy(-60, 244, controller.definitions.FastHybridEnemyDefinition);
		}
		private void SpawnHybridTankEnemy_Click(object sender, RoutedEventArgs e)
		{
			controller.CreateEnemy(-60, 244, controller.definitions.HybridTankEnemyDefinition);
		}
		private void SpawnBossEnemy_Click(object sender, RoutedEventArgs e)
		{
			controller.CreateEnemy(-60, 244, controller.definitions.LevelBossEnemyDefinition);
		}
		private void GiveInfiniteMoney_Click(object sender, RoutedEventArgs e)
		{
			controller.playerData.Money = 9999999;
		}
		private void WinGame_Click(object sender, RoutedEventArgs e)
		{
			controller.GameWon = true;
		}
		private void LoseGame_Click(object sender, RoutedEventArgs e)
		{
			TowerModificationMenu.IsOpen = true;
		}
		// ############################# DEV CONTROLS #####################################

		
	}
}
