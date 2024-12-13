using System.Collections.Generic;
using Game.Save;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay
{
    public class GameplayManager : IInitializable
    {
        private const string _saveID = nameof(GameplayManager);
        
        private List<GameplayStatsData> _gameplayStats = new();
        private GameplayController _gameplayController;

        private SaveManager _saveManager;
        
        public List<GameplayStatsData> GameplayStats => _gameplayStats;
        public GameplayController GameplayController => _gameplayController;


        [Inject]
        private void Install(SaveManager saveManager)
        {
            _saveManager = saveManager;
        }
        
        
        public void Initialize()
        {
            if (_saveManager.TryGetData<List<GameplayStatsData>>(_saveID, out var saveData))
            {
                _gameplayStats = saveData;
            }
        }
        
        public void RegisterController(GameplayController gameplayController)
        {
            _gameplayController = gameplayController;
        }

        public void UnregisterController(GameplayController gameplayController)
        {
            if (_gameplayController == gameplayController)
                _gameplayController = null;
        }

        public void AddStats(GameplayStatsData gameplayStatsData)
        {
            _gameplayStats.Add(gameplayStatsData);
            _saveManager.SetData(_saveID, _gameplayStats);
        }
    }
}