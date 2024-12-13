using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Data
{
    public abstract class DataConfig : SerializedScriptableObject
    {
        #if UNITY_EDITOR
        public abstract DataScriptable[] SystemData { get; }
        #endif
    }

    public abstract class DataConfig<TDataScriptable> : DataConfig where TDataScriptable : DataScriptable
    {
        [SerializeField] protected TDataScriptable[] _datas;
        public TDataScriptable[] Datas => _datas;

        
#if UNITY_EDITOR
        public override DataScriptable[] SystemData => Datas;


        [Button]
        private void FindAllData()
        {
            _datas = FindAssetForType<TDataScriptable>();

        }
        
        protected T[] FindAssetForType<T>() where T : Object
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name);
            
            T[] assets = new T[guids.Length];
            
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                    
                T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                
                assets[i] = asset;
            }
            
            return assets;
        }
        
        protected T[] FindAssetForMonoType<T>() where T : MonoBehaviour
        {
            string[] assetPaths = UnityEditor.AssetDatabase.GetAllAssetPaths();
            
            List<T> assets = new List<T>();

            foreach (var assetPath in assetPaths)
            {
                GameObject asset = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (asset != null && asset.TryGetComponent(out T assetComponent))
                {
                    assets.Add(assetComponent);
                }
            }
            return assets.ToArray();
        }
#endif
    }
}