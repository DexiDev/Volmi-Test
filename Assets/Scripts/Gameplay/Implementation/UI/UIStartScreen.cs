using Cysharp.Threading.Tasks;
using Game.Loadings;
using Game.UI;
using VContainer;

namespace Game.Gameplay.UI
{
    public class UIStartScreen : UIScreen
    {
        private GameplayManager _gameplayManager;
        private LoadingManager _loadingManager;

        private GameplayController _gameplayController;
        
        [Inject]
        private void Install(GameplayManager gameplayManager, LoadingManager loadingManager)
        {
            _gameplayManager = gameplayManager;
            _loadingManager = loadingManager;
        }

        private async void OnEnable()
        {
            await UniTask.WaitWhile(() => _loadingManager.IsLoading);
            
            _gameplayController = _gameplayManager.GameplayController;
            
            _gameplayController.IsRunningField.OnChanged += IsRunningChangeHandle;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _gameplayController.IsRunningField.OnChanged -= IsRunningChangeHandle;
        }

        private void IsRunningChangeHandle(bool isRunning)
        {
            gameObject.SetActive(false);
        }
    }
}