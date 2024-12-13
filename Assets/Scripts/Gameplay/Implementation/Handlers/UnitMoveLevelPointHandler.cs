using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Characters;
using Game.Core;
using Game.Levels.Fields;
using UnityEngine;

namespace Game.Gameplay.Handlers
{
    public class UnitMoveLevelPointHandler : IHandler<UnitController>
    {
        [SerializeField] private bool _canXDirection;
        [SerializeField] private bool _canYDirection;
        [SerializeField] private bool _canZDirection;
        [SerializeField] private float _threashold = 0.01f;
        
        private CancellationTokenSource _cancellationToken;
        private LevelPointField _levelPointField;
        
        private void Awake()
        {
            _levelPointField = _targetData.GetDataField<LevelPointField>(true);
        }

        private void OnEnable()
        {
            _levelPointField.OnChanged += PointChangeHandler;
        }
        
        private void OnDisable()
        {
            _levelPointField.OnChanged -= PointChangeHandler;
        }
        
        private async void PointChangeHandler(Vector3 point)
        {
            _cancellationToken?.Cancel();
            _cancellationToken = new CancellationTokenSource();

            var token = _cancellationToken.Token;
            try
            {
                Vector3 direction = GetDirection();
                while (direction.sqrMagnitude > _threashold && !token.IsCancellationRequested)
                {
                    _targetData.Move(direction * Time.deltaTime);
                    
                    await UniTask.Yield(cancellationToken: token);
                    
                    direction = GetDirection();
                }
            }
            catch (OperationCanceledException)
            {
                
            }
        }

        private Vector3 GetDirection()
        {
            var targetPosition = new Vector3(
                _canXDirection ? _levelPointField.Value.x : _targetData.transform.position.x,
                _canYDirection ? _levelPointField.Value.y : _targetData.transform.position.y,
                _canZDirection ? _levelPointField.Value.z : _targetData.transform.position.z);

            var direction =  targetPosition - _targetData.transform.position;

            return direction;
        }
    }
}