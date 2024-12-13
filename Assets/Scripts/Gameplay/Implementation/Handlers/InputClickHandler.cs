using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Game.Gameplay.Handlers
{
    public class InputClickHandler : IHandler<InputActionReference>
    {
        [SerializeField] private UnityEvent _onClick;
        
        private InputAction _inputAction;
        
        private void Awake()
        {
            _inputAction = _targetData.action;
        }

        private void OnEnable()
        {
            _inputAction.Enable();
            _inputAction.started += InputStartedHandle;
        }

        private void OnDisable()
        {
            _inputAction.Disable();
            _inputAction.started -= InputStartedHandle;
        }
        
        private void InputStartedHandle(InputAction.CallbackContext obj)
        {
            _onClick?.Invoke();
        }
    }
}