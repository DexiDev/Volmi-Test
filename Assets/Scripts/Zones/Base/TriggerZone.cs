using System;
using System.Collections.Generic;
using System.Linq;
using Game.Assets;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Zones
{
    public abstract class TriggerZone<TController> : SerializedMonoBehaviour, IAsset
    {
        [SerializeField] protected UnityEvent _enterAction;
        [SerializeField] protected UnityEvent _exitAction;
        [SerializeField] private AssetGroupData _assetGroupData;

        protected Dictionary<Collider, TController> _controllers = new();
        
        public Dictionary<Collider, TController> Controllers => _controllers;
        public AssetGroupData AssetGroup => _assetGroupData;
        public IAsset Contract { get; set; }
        public GameObject Instance => gameObject;
        
        public event Action OnEnter;
        public event Action OnExit;
        public event Action<TController> OnControllerEnter;
        public event Action<TController> OnControllerExit;
        public event Action<IAsset> OnReleased;

        protected virtual  void OnEnable()
        {
            
        }

        protected virtual void OnDisable()
        {
            _controllers.ToArray().ForEach(controller => Exit(controller.Key, controller.Value));
            _controllers.Clear();
            OnReleased?.Invoke(this);
        }

        private void OnCollisionEnter(Collision collision)
        {
            var collider = collision.collider;
            if (!_controllers.ContainsKey(collider) && TryGetController(collider, out TController controller))
            {
                Enter(collider, controller);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            var collider = other.collider;
            if (_controllers.TryGetValue(collider, out var controller))
            {
                Exit(collider, controller);
            }
        }

        public void TryTrigger(Collider collider)
        {
            if(TryGetController(collider, out TController controller))
            {
                Trigger(controller);
            }
        }

        protected void OnTriggerEnter(Collider collider)
        {
            if (!_controllers.ContainsKey(collider) && TryGetController(collider, out TController controller))
            {
                Enter(collider, controller);
            }
        }

        protected void OnTriggerExit(Collider collider)
        {
            if (_controllers.TryGetValue(collider, out var controller))
            {
                Exit(collider, controller);
            }
        }

        private void LateUpdate()
        {
            if (_controllers.Count > 0)
            {
                var controllers = _controllers.Where(controller => !controller.Key.enabled);
                if (controllers.Any())
                {
                    controllers.ToArray().ForEach(controller => Exit(controller.Key, controller.Value));   
                }
            }
        }

        protected virtual void Enter(Collider collider, TController controller)
        {
            _controllers.Add(collider, controller);
            OnEnter?.Invoke();
            OnControllerEnter?.Invoke(controller);
            _enterAction?.Invoke();
            
            Trigger(controller);
        }
        
        protected virtual void Trigger(TController controller)
        {
            
        }

        protected virtual void Exit(Collider collider, TController controller)
        {
            _controllers.Remove(collider);
            OnExit?.Invoke();
            OnControllerExit?.Invoke(controller);
            _exitAction?.Invoke();
        }

        private bool TryGetController(Collider collider, out TController controller)
        {
            controller = collider.GetComponentInParent<TController>();
            
            return controller != null;
        }
        
        public virtual bool HasControllers()
        {
            return _controllers.Count > 0;
        }
    }
}