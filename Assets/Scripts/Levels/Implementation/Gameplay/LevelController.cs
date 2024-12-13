using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Assets;
using Game.Gameplay.Controllers;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Game.Levels.Gameplay
{
    [DisallowMultipleComponent]
    public class LevelController : SerializedMonoBehaviour
    {
        [SerializeField, TabGroup("Base")] private PlayerController _playerController;
        [SerializeField, TabGroup("Base")] private Transform[] _lanePoints;
        [SerializeField, TabGroup("Parameters")] private int _startCountNodes;
        
        private LevelManager _levelManager;
        private AssetsManager _assetsManager;

        private LevelData _levelData;
        private CancellationTokenSource _cancellationTokenLoad; 
            
        private bool _isRunning;
        private List<LevelNode> _levelNodes = new List<LevelNode>();
        
        public LevelData LevelData => _levelData;
        public Transform[] LanePoints => _lanePoints;
        
        [Inject]
        private void Install(LevelManager levelManager, AssetsManager assetsManager)
        {
            _levelManager = levelManager;
            _assetsManager = assetsManager;
        }

        private void Awake()
        {
            _levelData = _levelManager.GetData(_levelManager.CurrentLevelID);
        }
        
        private void OnEnable()
        {
            InitializeNodes();
        }

        private void OnDisable()
        {
            ClearNodes();
        }
        
        private async void InitializeNodes()
        {
            ClearNodes();

            _levelNodes ??= new();

            _cancellationTokenLoad = new();
            
            for (int i = 0; i < _startCountNodes; i++) await AddNode(_cancellationTokenLoad.Token);

            _cancellationTokenLoad = null;
        }
        
        private async UniTask<LevelNode> AddNode(CancellationToken cancellationToken)
        {
            var nodesContract = _levelData.NodesContract;
            
            if (nodesContract != null && nodesContract.Any())
            {
                Vector3 position = Vector3.zero;
                if (_levelNodes != null && _levelNodes.Any())
                {
                    LevelNode lastLevelNode = _levelNodes.Last();
                    position = lastLevelNode.transform.position;
                    position += lastLevelNode.Offset;
                }

                LevelNode levelNodeContract = nodesContract[Random.Range(0, nodesContract.Length)];

                if (levelNodeContract != null)
                {
                    try
                    {
                        LevelNode levelNode = await _assetsManager.GetAssetAsync(levelNodeContract, position, Quaternion.identity, transform, cancellationToken);

                        levelNode.Initialize(_levelData);
                        
                        levelNode.OnVisible += OnLevelNodeVisible;
                        levelNode.OnReleased += OnLevelNodeReleased;

                        _levelNodes ??= new();

                        _levelNodes.Add(levelNode);

                        return levelNode;
                    }
                    catch (OperationCanceledException)
                    {
                        
                    }
                }
            }
            return null;
        }

        private void ClearNodes()
        {
            if (_levelNodes == null) return;
            _levelNodes?.ForEach(levelNode => levelNode.OnVisible -= OnLevelNodeVisible);
            _levelNodes?.ToArray().ForEach(levelNode => levelNode.gameObject.SetActive(false));
            _levelNodes?.Clear();
        }

        private void OnLevelNodeReleased(IAsset asset)
        {
            asset.OnReleased -= OnLevelNodeReleased;
            
            if (asset is LevelNode levelNode)
            {
                levelNode.OnVisible -= OnLevelNodeVisible;
                _levelNodes.Remove(levelNode);
            }
            
        }

        private async void OnLevelNodeVisible(LevelNode levelNode, bool isVisible)
        {
            if (_playerController == null || _playerController.IsDestroyed()) return;

            if(!isVisible)
            {
                var levelNodePosition = levelNode.transform.position + levelNode.Offset;
                if (levelNodePosition.z < _playerController.transform.position.z)
                {
                    levelNode.gameObject.SetActive(false);
                    
                    await UniTask.WaitWhile(() => _cancellationTokenLoad != null);

                    _cancellationTokenLoad = new();
                    await AddNode(_cancellationTokenLoad.Token);
                    _cancellationTokenLoad = null;
                }
            }
        }
    }
}