using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Loadings;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;


namespace Game.Assets
{
    public class AssetsManager : DataManager<AssetGroupData, AssetConfig>
    {
        private IObjectResolver _objectResolver;
        
        private Transform _container;
        private HashSet<IAsset> _assetsActive = new();
        private Dictionary<IAsset, HashSet<IAsset>> _assetsPool = new();
        private List<string> _queueAsyncOperations = new();
        
        public bool IsLoading => _queueAsyncOperations.Count > 0;
        
        [Inject]
        private void Install(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        protected override void Initialized()
        {
            base.Initialized();
            CreateContainer();
        }

        private void CreateContainer()
        {
            var container = new GameObject("Asset Container")
            {
                active = false
            };
            
            Object.DontDestroyOnLoad(container);
            
            _container = container.transform;
        }

        private T GetAssetPool<T>(T assetContract, Vector3 position, Quaternion rotation, Transform parent) where T : Object, IAsset
        {
            T assetInstance = null;

            if (_assetsPool.TryGetValue(assetContract, out var poolList))
            {
                assetInstance = poolList.FirstOrDefault() as T;
            }

            if (assetInstance != null)
            {
                _assetsPool[assetContract].Remove(assetInstance);

                if (assetInstance.Instance.IsUnityNull() || assetInstance.Instance.IsDestroyed())
                {
                    assetInstance = GetAsset<T>(assetContract, position, rotation, parent);
                    return assetInstance;
                }

                assetInstance.Instance.transform.position = position;
                assetInstance.Instance.transform.rotation = rotation;
                assetInstance.Instance.transform.SetParent(parent, true);
                assetInstance.Instance.gameObject.SetActive(true);
            }

            return assetInstance;
        }

        public T GetAsset<T>(T prefab, Transform parent) where T : Object, IAsset
        {
            if(parent != null) return GetAsset<T>(prefab, parent.position, parent.rotation, parent);
            else return GetAsset<T>(prefab, Vector3.zero, Quaternion.identity, parent);
        }

        public T GetAsset<T>(T assetContract, Vector3 position, Quaternion rotation, Transform parent) where T : Object, IAsset
        {
            T assetInstance = GetAssetPool<T>(assetContract, position, rotation, parent);
            
            if (assetInstance == null)
            {
                assetInstance = Object.Instantiate(assetContract, position, rotation, _container);
                
                assetInstance.Contract = assetContract;
                
                _objectResolver.InjectGameObject(assetInstance.Instance);

                assetInstance.Instance.transform.SetParent(parent, true);
            }

            assetInstance.OnReleased += OnAssetRelease;
            
            _assetsActive.Add(assetInstance);
            
            return assetInstance;
        }
        public async UniTask<T> GetAssetAsync<T>(T assetContract, Vector3 position, Quaternion rotation, Transform parent, CancellationToken cancellationToken = default) where T : Object, IAsset
        {
            T assetInstance = GetAssetPool<T>(assetContract, position, rotation, parent);
            
            if (assetInstance == null)
            {
                AsyncInstantiateOperation<T> asyncOperation = null;
                string token = null;
                try
                {
                    do token = GenerateRandomToken();
                    while (_queueAsyncOperations.Any(queue => queue == token));
                    
                    _queueAsyncOperations.Add(token);
                    
                    await UniTask.WaitWhile(()=> _queueAsyncOperations.First() != token, cancellationToken: cancellationToken);
                    
                    asyncOperation = Object.InstantiateAsync(assetContract, _container, position, rotation);
                    
                    await UniTask.WaitWhile(() => !asyncOperation.isDone, cancellationToken: cancellationToken);
                    
                    assetInstance = asyncOperation.Result[0] as T;

                    if (assetInstance == null) return null;

                    assetInstance.Contract = assetContract;

                    _objectResolver.InjectGameObject(assetInstance.Instance);

                    assetInstance.Instance.transform.SetParent(parent, true);

                    if(token != null) _queueAsyncOperations.Remove(token);
                }
                catch (OperationCanceledException)
                {
                    asyncOperation?.Cancel();

                    if (token != null) _queueAsyncOperations.Remove(token);
                    
                    return null;
                }
            }

            if (assetInstance != null)
            {
                assetInstance.OnReleased += OnAssetRelease;

                _assetsActive.Add(assetInstance);
            }

            return assetInstance;
        }

        public static string GenerateRandomToken(int length = 16)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            System.Random random = new System.Random();
            char[] key = new char[length];

            for (int i = 0; i < length; i++)
            {
                key[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(key);
        }
        
        private void OnAssetRelease(IAsset assetInstance)
        {
            if (!_assetsActive.Contains(assetInstance)) return;
            
            assetInstance.OnReleased -= OnAssetRelease;

            var assetContract = assetInstance.Contract;
            
            _assetsActive.Remove(assetInstance);
            
            if (!assetInstance.Instance.IsDestroyed())
            {
                if (assetInstance.AssetGroup != null)
                {
                    var count = _assetsPool?
                        .Where(poolAsset => poolAsset.Value?.Count > 0)
                        .SelectMany(poolAsset => poolAsset.Value)
                        .FilterCast<IAsset>()
                        .Count(poolAsset => poolAsset.AssetGroup == assetInstance.AssetGroup);

                    if (assetInstance.AssetGroup.PoolLimit <= count)
                    {
                        Object.Destroy(assetInstance.Instance);
                        return;
                    }
                }

                _assetsPool.TryAdd(assetContract, new HashSet<IAsset>());

                _assetsPool[assetContract].Add(assetInstance);

                if (assetInstance.Instance.activeSelf)
                {
                    assetInstance.Instance.SetActive(false);
                }
            }
        }

        public void ReleaseAsset(IAsset IAsset)
        {
            OnAssetRelease(IAsset);
        }
    }
}