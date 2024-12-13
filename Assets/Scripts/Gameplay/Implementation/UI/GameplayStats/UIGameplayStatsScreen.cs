using Game.UI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Gameplay.UI.GameplayStats
{
    public class UIGameplayStatsScreen : UIScreen
    {
        [SerializeField] private UIGameplayStat _uiGameplayStatContract;
        [SerializeField] private Transform _statsContainer;
        
        private UIManager _uiManager;
        private GameplayManager _gameplayManager;

        [Inject]
        private void Install(UIManager uiManager, GameplayManager gameplayManager)
        {
            _uiManager = uiManager;
            _gameplayManager = gameplayManager;
        }

        private void OnEnable()
        {
            _gameplayManager.GameplayStats?.ForEach(stats =>
            {
                var uiGameplayStat = _uiManager.ShowElement(_uiGameplayStatContract, _statsContainer);
                uiGameplayStat.Initialize(stats);
            });
            
            foreach (var layoutGroup in gameObject.GetComponentsInChildren<LayoutGroup>())
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
            }
        }
    }
}