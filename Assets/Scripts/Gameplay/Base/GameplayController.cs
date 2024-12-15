using System;
using System.Collections.Generic;
using Game.Gameplay.Controllers;
using Game.Gameplay.Fields;
using Game.Gameplay.UI;
using Game.Inputs.Handlers;
using Game.Items;
using Game.Levels.Fields;
using Game.Levels.Gameplay;
using Game.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using VContainer;
using Random = UnityEngine.Random;

namespace Game.Gameplay
{
    public class GameplayController : SerializedMonoBehaviour, IInputSwipeAction
    {
        [SerializeField] private LevelController _levelController;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private UnityEvent _onStartGame;
        [SerializeField] private UnityEvent _onStopGame;
        [SerializeField] private UILoseScreen _uiLoseScreenContract;
        [SerializeField] private Dictionary<bool, MonoBehaviour> _runningComponents = new();
        
        private int _levelPointIndex;
        private GameplayScoreData _gameplayScoreData;
        private LevelPointField _levelPointField;
        private IsRunningField _isRunningField;
        
        private UIManager _uiManager;
        private GameplayManager _gameplayManager;
        private ItemsManager _itemsManager;
        
        public LevelController LevelController => _levelController;
        public PlayerController PlayerController => _playerController;
        public IsRunningField IsRunningField => _isRunningField;
        public GameplayScoreData GameplayScoreData => _gameplayScoreData;
        
        public event Action<GameplayScoreData> OnScoreUpdated;

        [Inject]
        private void Install(GameplayManager gameplayManager, ItemsManager itemsManager, UIManager uiManager)
        {
            _uiManager = uiManager;
            _gameplayManager = gameplayManager;
            _itemsManager = itemsManager;
        }

        private void Awake()
        {
            _levelPointField = _playerController.GetDataField<LevelPointField>(true);
            _isRunningField = _playerController.GetDataField<IsRunningField>(true);
            RandomStartPoint();
        }

        private void Start()
        {
            _gameplayScoreData = new GameplayScoreData(_levelController.LevelData.ID);
        }

        private void OnEnable()
        {
            _gameplayManager.RegisterController(this);
        }

        private void OnDisable()
        {
            StopGame();
            _gameplayManager.UnregisterController(this);
        }

        private void RandomStartPoint()
        {
            int levelPointIndex = Random.Range(0, _levelController.LanePoints.Length);
            SetLevelPoint(levelPointIndex);
            _playerController.transform.position = _levelPointField.Value;
        }

        private void SetLevelPoint(int index)
        {
            if (index >= 0 && index < _levelController.LanePoints.Length)
            {
                _levelPointIndex = index;
                Transform point = _levelController.LanePoints[_levelPointIndex];
                _levelPointField.SetValue(point.position);
            }
        }

        public void StartGame()
        {
            if (!_isRunningField.Value)
            {
                _itemsManager.OnItemAdded += ItemAddedHandler;
                _isRunningField.SetValue(true);
                _onStartGame?.Invoke();
            }
        }

        public void StopGame()
        {
            if (_isRunningField.Value)
            {
                _itemsManager.OnItemAdded -= ItemAddedHandler;
                _isRunningField.SetValue(false);
                _gameplayManager.AddScore(_gameplayScoreData);
                _onStopGame?.Invoke();

                _uiManager.ShowElement(_uiLoseScreenContract);
            }
        }

        private void ItemAddedHandler(string itemID, int count)
        {
            _gameplayScoreData.AddItem(itemID, count);
            OnScoreUpdated?.Invoke(_gameplayScoreData);
        }

        public void Swipe(bool isLeft)
        {
            if (isLeft) SetLevelPoint(_levelPointIndex - 1);
            else SetLevelPoint(_levelPointIndex + 1);
        }
    }
}