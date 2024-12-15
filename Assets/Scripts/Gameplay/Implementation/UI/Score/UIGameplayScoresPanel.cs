using Game.UI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Gameplay.UI.GameplayScore
{
    public class UIGameplayScoresPanel : UIScreen
    {
        [SerializeField] private UIGameplayScore _uiGameplayScoreContract;
        [SerializeField] private Transform _scoresContainer;
        
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
            _gameplayManager.GameplayScoreDatas?.ForEach(scoreData =>
            {
                var uiGameplayStat = _uiManager.ShowElement(_uiGameplayScoreContract, _scoresContainer);
                uiGameplayStat.Initialize(scoreData);
            });
            
            foreach (var layoutGroup in gameObject.GetComponentsInChildren<LayoutGroup>())
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
            }
        }
    }
}