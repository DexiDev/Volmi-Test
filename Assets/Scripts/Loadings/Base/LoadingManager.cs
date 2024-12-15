using System;
using Cysharp.Threading.Tasks;
using Game.Assets;
using Game.Data;
using Game.Loadings.UI;
using UnityEngine.SceneManagement;
using VContainer;
using Object = UnityEngine.Object;

namespace Game.Loadings
{
    public class LoadingManager : DataManager<LoadingData, LoadingConfig>
    {
        private AssetsManager _assetsManager;
        
        private bool _isLoading;
        private UILoadingScreen _uiLoadingScreen;
        private LoadingData _currentLoadingData;

        public bool IsLoading => _isLoading;
        public string CurrentLoadingID => _currentLoadingData?.ID;

        [Inject]
        private void Install(AssetsManager assetsManager)
        {
            _assetsManager = assetsManager;
        }
        
        protected override void Initialized()
        {
            base.Initialized();
            
            _uiLoadingScreen = _assetsManager.GetAsset(_config.LoadingScreen, null);
            
            Object.DontDestroyOnLoad(_uiLoadingScreen);
            
            _uiLoadingScreen.SetActive(false);
        }

        public void LoadMenu()
        {
            Load(_config.MetaLoadingData);
        }

        public void Load(string loadingDataID)
        {
            var loadingData = GetData(loadingDataID);

            if (loadingData != null)
            {
                Load(loadingData);
            }
        }

        private async void Load(LoadingData loadingData)
        {
            if (!_isInitialize) await UniTask.WaitWhile(() => !_isInitialize);

            _isLoading = true;
            
            var startTime = DateTime.Now;
            
            _uiLoadingScreen.SetProgress(0f);
            
            _uiLoadingScreen.SetActive(true);
            
            if (loadingData != null && loadingData.LoadingSettings != null)
                _uiLoadingScreen.AddTask(loadingData.LoadingSettings.Length);

            if (loadingData.LoadingSettings is { Length: > 0 } &&
                loadingData.LoadingSettings[0].LoadSceneMode == LoadSceneMode.Additive)
            {
                if (_currentLoadingData != null && _currentLoadingData.LoadingSettings != null)
                {
                    _uiLoadingScreen.AddTask(_currentLoadingData.LoadingSettings.Length);

                    foreach (var loadingSettings in _currentLoadingData.LoadingSettings)
                    {
                        var sceneOperation = SceneManager.UnloadSceneAsync(loadingSettings.SceneAsset.SceneName);

                        while (!sceneOperation.isDone)
                        {
                            _uiLoadingScreen.SetProgress(sceneOperation.progress);
                            await UniTask.Yield();
                        }

                        _uiLoadingScreen.CompletedTask();
                    }
                }
            }

            _currentLoadingData = loadingData;

            if (_currentLoadingData != null && _currentLoadingData.LoadingSettings != null)
            {
                foreach (var loadingSettings in _currentLoadingData.LoadingSettings)
                {
                    await UniTask.WaitWhile(() => _assetsManager.IsLoading);
                    
                    var sceneOperation = SceneManager.LoadSceneAsync(loadingSettings.SceneAsset.SceneName, loadingSettings.LoadSceneMode);

                    while (!sceneOperation.isDone)
                    {
                        _uiLoadingScreen.SetProgress(sceneOperation.progress);
                        await UniTask.Yield();
                    }

                    await UniTask.WaitWhile(() => _assetsManager.IsLoading);
                    
                    _uiLoadingScreen.CompletedTask();

                    if (loadingSettings.IsActive)
                    {
                        var scene = SceneManager.GetSceneByName(loadingSettings.SceneAsset.SceneName);
                        SceneManager.SetActiveScene(scene);
                    }
                }
            }

            if (loadingData.MinDurationLoading > 0f)
            {
                var time = DateTime.Now - startTime;

                var targetTime = TimeSpan.FromSeconds(loadingData.MinDurationLoading);
                
                if (time < targetTime)
                {
                    var delay = targetTime - time;
                    await UniTask.Delay(delay);
                }
            }
            _uiLoadingScreen.SetActive(false);
            
            _isLoading = false;
        }
    }
}