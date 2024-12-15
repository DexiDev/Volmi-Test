using System;
using Game.Loadings;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace Game.Renderers
{
    [RequireComponent(typeof(Renderer))]
    public class RendererVisible : SerializedMonoBehaviour
    {
        [SerializeField] private Renderer _renderer;

        [Inject] private LoadingManager _loadingManager; 
        private bool _isVisible = true;

        public bool IsVisible => _isVisible;
        
        public event Action<bool> OnVisible;

        private bool _forceCheck;

#if UNITY_EDITOR
        [Button]
        public void InitializeComponent()
        {
            _renderer = GetComponent<Renderer>();
        }
#endif
        
        private void Awake()
        {
            if (_renderer == null)
            {
                _renderer = GetComponent<Renderer>();    
            }
        }

        protected virtual void OnEnable()
        {
            _forceCheck = true;
        }

        protected virtual void OnDisable()
        {
            SetVisible(false);
        }

        private void Update()
        {
            if (!_loadingManager.IsLoading && _forceCheck)
            {
                _forceCheck = false;
                SetVisible(_renderer.isVisible);
            }
        }

        protected virtual void OnBecameVisible()
        {
            SetVisible(true);
        }

        protected virtual  void OnBecameInvisible()
        {
            SetVisible(false);
        }

        public void SetVisible(bool isVisible)
        {
            if (_isVisible != isVisible)
            {
                _isVisible = isVisible;
                OnSetVisible(isVisible);
            }
        }

        protected virtual void OnSetVisible(bool isVisible)
        {
            OnVisible?.Invoke(isVisible);
        }
    }
}