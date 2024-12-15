using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Assets;
using Game.Gameplay.Controllers;
using Game.Loadings;
using Game.Renderers;
using Game.Zones;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Game.Levels.Gameplay
{
    public class LevelNode : SerializedMonoBehaviour, IAsset
    {
        [SerializeField, TabGroup("Assets")] private AssetGroupData _assetGroupData;
        [SerializeField, TabGroup("Obstacle")] private Transform _obstacleContainer;
        [SerializeField, TabGroup("Parameters")] private Vector3 _offset;
        [SerializeField, TabGroup("Parameters")] private int _laneCount = 3;
        [SerializeField, TabGroup("Components")] private BoxCollider _boxCollider;
        [SerializeField, TabGroup("Components")] private RendererVisible[] _rendererVisibles;
        
        private AssetsManager _assetsManager;
        private LoadingManager _loadingManager;

        private LevelData _levelData;
        private bool _isVisible;
        
        public Vector3 Offset => _offset;

        public bool IsVisible => _isVisible;
        public IAsset Contract { get; set; }
        public GameObject Instance => gameObject;
        public AssetGroupData AssetGroup => _assetGroupData;
        
        public event Action<IAsset> OnReleased;
        public event Action<LevelNode, bool> OnVisible;

        [Inject]
        private void Install(AssetsManager assetsManager, LoadingManager loadingManager)
        {
            _assetsManager = assetsManager;
            _loadingManager = loadingManager;
        }

        public void Initialize(LevelData levelData)
        {
            _levelData = levelData;
            GenerateObstacles();
        }
        
        private async void OnEnable()
        {
            _isVisible = _rendererVisibles.Any(rendererVisible => rendererVisible.IsVisible);
            await UniTask.WaitWhile(() => _loadingManager.IsLoading);

            _rendererVisibles?.ForEach(rendererVisible => rendererVisible.OnVisible += OnRendererVisible);
        }

        private void OnDisable()
        {
            _isVisible = false;
            _rendererVisibles?.ForEach(rendererVisible => rendererVisible.OnVisible -= OnRendererVisible);
            OnReleased?.Invoke(this);
        }

        private void OnRendererVisible(bool isVisible)
        {
            var visible=  _rendererVisibles.Any(rendererVisible => rendererVisible.IsVisible); 

            if (visible != _isVisible)
            {
                _isVisible = visible;
                OnVisible?.Invoke(this, IsVisible);
            }
        }

        private async void GenerateObstacles()
        {
            if (_levelData.ObstaclesContract == null || _levelData.ObstaclesContract.Length == 0) return;
            
            Vector3 blockSize = _boxCollider.size;
            Vector3 blockCenter = _boxCollider.transform.TransformPoint(_boxCollider.center);

            float blockStartZ = blockCenter.z - blockSize.z / 2;
            float blockEndZ = blockCenter.z + blockSize.z / 2;

            float laneWidth = blockSize.x / _laneCount;

            float blockStartX = blockCenter.x - blockSize.x / 2;
            float laneStartOffset = blockStartX + laneWidth / 2;

            float obstacleHeight = blockCenter.y + blockSize.y / 2;

            float currentZ = blockStartZ;

            var randomFirstSpacing = Random.Range(_levelData.MinMaxObstacleSpacing.x, _levelData.MinMaxObstacleSpacing.y / 2f);
            
            currentZ += randomFirstSpacing;
            
            while (currentZ < blockEndZ)
            {
                Vector2 minMaxSpacing;
                
                TriggerZoneCollider<PlayerController, BoxCollider> contractAsset;
                
                if (Random.Range(0, 101) < _levelData.ChanceReward)
                {
                    contractAsset = _levelData.ItemRewards[Random.Range(0, _levelData.ItemRewards.Length)];
                    minMaxSpacing = _levelData.MinMaxRewardSpacing;
                }
                else
                {
                    contractAsset = _levelData.ObstaclesContract[Random.Range(0, _levelData.ObstaclesContract.Length)];
                    minMaxSpacing = _levelData.MinMaxObstacleSpacing;
                }
                
                float assetLength = contractAsset.Collider.size.z;
                
                var lane = Random.Range(0, _laneCount);
                float lanePositionX = laneStartOffset + lane * laneWidth;
                
                Vector3 assetPosition = new Vector3(lanePositionX, obstacleHeight, currentZ + assetLength / 2);
                
                await _assetsManager.GetAssetAsync(contractAsset, assetPosition, Quaternion.identity, _obstacleContainer);
                
                var randomSpacing = Random.Range(minMaxSpacing.x, minMaxSpacing.y);
                currentZ += assetLength + randomSpacing;
            }
        }
    }
}