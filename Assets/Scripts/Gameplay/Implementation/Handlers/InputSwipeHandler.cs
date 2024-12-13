using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Gameplay.Handlers
{
    public interface IInputSwipeAction
    {
        void Swipe(bool isLeft);
    }
    
    public class InputSwipeHandler : IHandler<IInputSwipeAction>
    {
        [SerializeField] private InputActionReference _inputActionClickContract;
        [SerializeField] private InputActionReference _inputActionSwipeContract;
        [SerializeField] private float _swipeThreshold = 100f;

        private InputAction _inputActionClick;
        private InputAction _inputActionSwipe;
        private CancellationTokenSource _cancellationToken;
        
        private void Awake()
        {
            _inputActionClick = _inputActionClickContract.action;
            _inputActionSwipe = _inputActionSwipeContract.action;
            
        }

        private void OnEnable()
        {
            _inputActionClick.Enable();
            _inputActionSwipe.Enable();
            _inputActionClick.started += InputStartedHandle;
            _inputActionClick.canceled += InputCanceledHandle;
        }

        private void OnDisable()
        {
            _inputActionClick.Disable();
            _inputActionSwipe.Disable();
            _inputActionClick.started -= InputStartedHandle;
            _inputActionClick.canceled -= InputCanceledHandle;
        }
        
        private void InputStartedHandle(InputAction.CallbackContext obj)
        {
            var startPoint = _inputActionSwipe.ReadValue<Vector2>();
            InputUpdate(startPoint);
        }

        private void InputCanceledHandle(InputAction.CallbackContext obj)
        {
            _cancellationToken?.Cancel();
        }

        private async void InputUpdate(Vector2 startPoint)
        {
            _cancellationToken?.Cancel();
            _cancellationToken = new CancellationTokenSource();
            var token = _cancellationToken.Token;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var currentPoint = _inputActionSwipe.ReadValue<Vector2>();
                    
                    Vector2 swipeDirection = currentPoint - startPoint;

                    if (swipeDirection.magnitude > _swipeThreshold)
                    {
                        if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
                        {
                            if (swipeDirection.x > 0) _targetData.Swipe(false);

                            else _targetData.Swipe(true);


                            return;
                        }
                    }
                    await UniTask.Yield(token);
                }
            }
            catch (OperationCanceledException)
            {
                
            }
        }
    }
}