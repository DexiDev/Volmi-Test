using System.Collections.Generic;
using Game.Save;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay
{
    public class GameplayManager : IInitializable
    {
        private const string _saveID = nameof(GameplayManager);
        
        private List<GameplayScoreData> _gameplayScoreDatas = new();
        private GameplayController _gameplayController;

        private SaveManager _saveManager;
        
        public List<GameplayScoreData> GameplayScoreDatas => _gameplayScoreDatas;
        public GameplayController GameplayController => _gameplayController;


        [Inject]
        private void Install(SaveManager saveManager)
        {
            _saveManager = saveManager;
        }
        
        
        public void Initialize()
        {
            if (_saveManager.TryGetData<List<GameplayScoreData>>(_saveID, out var saveData))
            {
                _gameplayScoreDatas = saveData;
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

        public void AddScore(GameplayScoreData gameplayScoreData)
        {
            _gameplayScoreDatas.Add(gameplayScoreData);
            _saveManager.SetData(_saveID, _gameplayScoreDatas);
        }
    }
}