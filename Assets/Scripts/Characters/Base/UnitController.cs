using System;
using Game.Assets;
using Game.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Characters
{
    [DisallowMultipleComponent]
    public class UnitController : DataController, ICharacter
    {
        [SerializeField, TabGroup("Parameters")] protected float _speed;
        [SerializeField, TabGroup("Parameters")] protected float _speedRotation;
        [SerializeField, TabGroup("Components")] protected Animator _animator;
        [SerializeField, TabGroup("Animation")] private string _isMovingKey = "IsMoving";
        [SerializeField, TabGroup("Animation")] private string _speedKey = "Speed";
        [field: SerializeField] public AssetGroupData AssetGroup { get; private set; }
        
        public IAsset Contract { get; set; }
        public GameObject Instance => gameObject;
        public Animator Animator => _animator;
        public Transform Transform => transform;

        public event Action<IAsset> OnReleased;
        
        protected virtual void Awake()
        {
            
        }

        protected virtual void OnEnable()
        {
            
        }

        protected virtual void OnDisable()
        {
            OnReleased?.Invoke(this);
        }

        protected virtual void OnDestroy()
        {
            
        }

        public virtual void Move(Vector3 direction)
        {
            if (direction == Vector3.zero) return;

            var moveDirection = direction * GetSpeed();
            
            transform.position += moveDirection;
        }

        public virtual float GetSpeed()
        {
            return _speed;
        }
        
        public virtual void Rotate(Vector3 direction)
        {
            if (direction == Vector3.zero) return;

            transform.localEulerAngles += direction * _speedRotation;
        }

        public virtual void SetRotation(Quaternion targetRotation)
        {
            transform.rotation = targetRotation;
        }
        
        public virtual void SetMotionAnimation(bool isMoving, float speed = 0f)
        {
            if (_animator == null) return;
            
            _animator.SetBool(_isMovingKey, isMoving);
            _animator.SetFloat(_speedKey, speed);
        }
        
        public void SetAnimator(Animator animator)
        {
            _animator = animator;
        }
    }
}