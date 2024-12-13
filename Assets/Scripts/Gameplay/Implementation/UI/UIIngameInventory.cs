using Cysharp.Threading.Tasks;
using Game.Data.Attributes;
using Game.Items;
using Game.Loadings;
using Game.UI;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Gameplay.UI
{
    public class UIIngameInventory : UIElement
    {
        private struct ItemInfo
        {
            public Image Icon;
            [DataID(typeof(ItemsConfig))] public string ID;
        }
        
        [SerializeField] private ItemInfo[] _itemIDs;
        [SerializeField] private TMP_Text _textField;
        
        private GameplayStatsData _gameplayStatsData;
        
        private ItemsManager _itemsManager;
        private LoadingManager _loadingManager;
        private GameplayManager _gameplayManager;
        private GameplayController _gameplayController;
        
        [Inject]
        private void Install(ItemsManager itemsManager, LoadingManager loadingManager, GameplayManager gameplayManager)
        {
            _itemsManager = itemsManager;
            _loadingManager = loadingManager;
            _gameplayManager = gameplayManager;
        }
        
        private async void OnEnable()
        {
            await UniTask.WaitWhile(() => _loadingManager.IsLoading);
            
            _gameplayController = _gameplayManager.GameplayController;
            
            SetGameplayStatsData(_gameplayController.GameplayStatsData);

            UpdateIcons();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            SetGameplayStatsData(null);
        }

        private void UpdateIcons()
        {
            _itemIDs?.ForEach(item =>
            {
                var itemData = _itemsManager.GetData(item.ID);

                if (itemData != null)
                {
                    item.Icon.sprite = itemData.Icon;
                    item.Icon.color = itemData.IconColor;
                }
            });
        }
        
        private void SetGameplayStatsData(GameplayStatsData gameplayStatsData)
        {
            if (_gameplayStatsData != null)
            {
                _gameplayStatsData.OnItemChanged -= ItemChangeHandle;
            }

            _gameplayStatsData = gameplayStatsData;
            
            if (_gameplayStatsData != null)
            {
                _itemIDs?.ForEach(item => ItemChangeHandle(item.ID, 0));
                _gameplayStatsData.OnItemChanged += ItemChangeHandle;
            }
        }

        private void ItemChangeHandle(string itemID, int count)
        {
            int countItems = 0;

            _itemIDs?.ForEach(item =>
            {
                if (_gameplayStatsData != null && _gameplayStatsData.Items != null)
                {
                    if (_gameplayStatsData.Items.TryGetValue(item.ID, out int itemCount))
                    {
                        countItems += itemCount;
                    }
                }
            });

            _textField.text = countItems.ToString();
        }
    }
}