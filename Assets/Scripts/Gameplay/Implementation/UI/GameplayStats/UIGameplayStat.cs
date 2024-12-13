using Game.Levels;
using Game.UI;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.UI.GameplayStats
{
    public class UIGameplayStat : UIElement
    {
        [SerializeField] private TMP_Text _titleField;
        [SerializeField] private UIGameplayStatItem _uiGameplayStatItemContract;
        [SerializeField] private Transform _itemContainer;
        
        private GameplayStatsData _gameplayStatsData;
        
        private UIManager _uiManager;
        private LevelManager _levelManager;

        [Inject]
        private void Install(UIManager uiManager, LevelManager levelManager)
        {
            _uiManager = uiManager;
            _levelManager = levelManager;
        }
        
        public void Initialize(GameplayStatsData gameplayStatsData)
        {
            _gameplayStatsData = gameplayStatsData;
            
            SetTitle(gameplayStatsData.LevelID);

            gameplayStatsData.Items?.ForEach(item => AddItem(item.Key, item.Value));
        }

        private void SetTitle(string levelID)
        {
            var levelData = _levelManager.GetData(levelID);

            _titleField.text = levelData?.Name ?? string.Empty;
        }

        private void AddItem(string itemID, int amount)
        {
            var uiGameplayStatItem = _uiManager.ShowElement(_uiGameplayStatItemContract, _itemContainer);
            uiGameplayStatItem.Initialize(itemID, amount);
        }
    }
}