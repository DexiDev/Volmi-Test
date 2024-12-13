using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Data.Fields.Follow;
using UnityEngine;

namespace Game.Data.Handlers
{
    public abstract class FollowTargetHandler<TFollowTargetField, TDataController> : IHandler<TDataController> where TFollowTargetField : class, IFollowTargetField where TDataController : MonoBehaviour, IDataController
    {
        [SerializeField] protected Vector3 _offset;
        
        protected TFollowTargetField _followTargetField;
        private CancellationTokenSource _cancellationTokenSource;
        
        private void OnEnable()
        {
            SetField(_targetData.GetDataField<TFollowTargetField>());
            _targetData.OnFieldsChanged += OnTargetFieldsChangedHandler;
        }

        private void OnDisable()
        {
            _cancellationTokenSource?.Cancel();
            SetField(null);
            _targetData.OnFieldsChanged -= OnTargetFieldsChangedHandler;
        }

        private void OnTargetFieldsChangedHandler(IDataField dataField, bool isAdded)
        {
            if (dataField is TFollowTargetField followTargetField)
            {
                if (isAdded)
                {
                    SetField(followTargetField);
                }
                else if (_followTargetField == followTargetField)
                {
                    SetField(null);
                }
            }
        }

        private void SetField(TFollowTargetField followTargetField)
        {
            if (_followTargetField != null)
            {
                _followTargetField.OnDataChanged -= OnFieldDataChanged;
                _cancellationTokenSource?.Cancel();
            }
            
            _followTargetField = followTargetField;

            if (_followTargetField != null)
            {
                _followTargetField.OnDataChanged += OnFieldDataChanged;
                UpdateLoop();
            }
        }

        private void OnFieldDataChanged(IDataField dataField)
        {
            UpdateLoop();
        }

        private async void UpdateLoop()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new();

            var token = _cancellationTokenSource.Token;
            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    UpdatePosition();
                    await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, token);
                }
            }
            catch (OperationCanceledException) {}
        }

        protected virtual void UpdatePosition()
        {
            _targetData.transform.position = GetPosition() + _offset;
        }

        protected abstract Vector3 GetPosition();
    }
}