using Cysharp.Threading.Tasks;
using Game.Loadings;
using Game.UI;
using TMPro;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.UI
{
    public class UIGameplayScore : UIElement
    {
        [SerializeField] private TMP_Text _textField;
        
        private GameplayScoreData _gameplayScoreData;
        
        private LoadingManager _loadingManager;
        private GameplayManager _gameplayManager;
        private GameplayController _gameplayController;
        
        [Inject]
        private void Install(LoadingManager loadingManager, GameplayManager gameplayManager)
        {
            _loadingManager = loadingManager;
            _gameplayManager = gameplayManager;
        }
        
        private async void OnEnable()
        {
            await UniTask.WaitWhile(() => _loadingManager.IsLoading);
            
            _gameplayController = _gameplayManager.GameplayController;
            
            SetGameplayScoreData(_gameplayController.GameplayScoreData);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            SetGameplayScoreData(null);
        }
        
        private void SetGameplayScoreData(GameplayScoreData gameplayScoreData)
        {
            if (_gameplayScoreData != null)
            {
                _gameplayScoreData.OnItemChanged -= ItemChangeHandle;
            }

            _gameplayScoreData = gameplayScoreData;
            
            if (_gameplayScoreData != null)
            {
                ItemChangeHandle(null, 0);
                _gameplayScoreData.OnItemChanged += ItemChangeHandle;
            }
        }

        private void ItemChangeHandle(string itemID, int count)
        {
            _textField.text = _gameplayScoreData.GetItemsAllCount().ToString();
        }
    }
}