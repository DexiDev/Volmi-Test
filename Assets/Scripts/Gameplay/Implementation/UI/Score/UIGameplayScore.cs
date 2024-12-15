using Game.Inventory.UI;
using Game.Levels;
using Game.UI;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.UI.GameplayScore
{
    public class UIGameplayScore : UIElement
    {
        [SerializeField] private TMP_Text _titleField;
        [SerializeField] private UIItem _uiItemContract;
        [SerializeField] private Transform _itemContainer;
        
        private GameplayScoreData _gameplayScoreData;
        
        private UIManager _uiManager;
        private LevelManager _levelManager;

        [Inject]
        private void Install(UIManager uiManager, LevelManager levelManager)
        {
            _uiManager = uiManager;
            _levelManager = levelManager;
        }
        
        public void Initialize(GameplayScoreData gameplayScoreData)
        {
            _gameplayScoreData = gameplayScoreData;
            
            SetTitle(gameplayScoreData.LevelID);

            gameplayScoreData.Items?.ForEach(item => AddItem(item.Key, item.Value));
        }

        private void SetTitle(string levelID)
        {
            var levelData = _levelManager.GetData(levelID);

            _titleField.text = levelData?.Name ?? string.Empty;
        }

        private void AddItem(string itemID, int amount)
        {
            var uiGameplayStatItem = _uiManager.ShowElement(_uiItemContract, _itemContainer);
            uiGameplayStatItem.Initialize(itemID, amount);
        }
    }
}