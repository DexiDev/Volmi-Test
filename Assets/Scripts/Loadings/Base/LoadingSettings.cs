using System;
using Game.Loadings.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Loadings
{
    [Serializable]
    public class LoadingSettings
    {
        [SerializeField] private SceneField _sceneAsset;
        
        [SerializeField] private LoadSceneMode _loadSceneMode = LoadSceneMode.Additive;

        [SerializeField] private bool _isActive;
        
        public SceneField SceneAsset => _sceneAsset;

        public LoadSceneMode LoadSceneMode => _loadSceneMode;

        public bool IsActive => _isActive;
    }
}