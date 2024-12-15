using System;
using System.Linq;
using Game.Data;
using Game.Loadings;
using VContainer;

namespace Game.Levels
{
    public class LevelManager : DataManager<LevelData, LevelConfig>
    {
        private const string _saveID = nameof(LevelManager);
        
        private int _currentLevelLoop;
        private int _currentLevelIndex;
        
        private LoadingManager _loadingManager;

        public int CurrentLevelLoop => _currentLevelLoop;
        public string CurrentLevelID => _config.Datas[_currentLevelIndex]?.ID;

        public event Action<int> OnLevelChanged;
        public event Action<int> OnLevelLoopChanged;
        
        [Inject]
        private void Install(LoadingManager loadingManager)
        {
            _loadingManager = loadingManager;
        }

        public void IncreaseLevel()
        {
            _currentLevelLoop++;
            _currentLevelIndex++;

            if (_currentLevelIndex >= _datas.Count) _currentLevelIndex = _config.SkipForLoop;
            
            OnLevelChanged?.Invoke(_currentLevelIndex);
            OnLevelLoopChanged?.Invoke(_currentLevelLoop);
        }

        public string[] GetAvailableLevels()
        {
            return _datas.Keys.ToArray();
        }
        
        public void LoadCurrentLevel()
        {
            LoadLevel(CurrentLevelID);
        }

        public void LoadLevel(string levelID)
        {
            var levelData = GetData(levelID);
            if (levelData != null)
            {
                for (int i = 0; i < _config.Datas.Length; i++)
                {
                    if (_config.Datas[i] == levelData)
                    {
                        _currentLevelIndex = i;
                        break;
                    }
                }
                
                LoadLevel(levelData);
            }
        }

        private void LoadLevel(LevelData levelData)
        {
            if (levelData != null)
            {
                _loadingManager.Load(levelData.LoadingID);   
            }
        }
    }
}