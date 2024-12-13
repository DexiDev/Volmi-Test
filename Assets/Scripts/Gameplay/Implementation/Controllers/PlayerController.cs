using Game.Characters;
using Game.Data.Attributes.Fields;
using Game.Data.Fields;
using Game.Extensions.Core;
using Game.Gameplay.Fields;
using Game.Gameplay.TriggerZones;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Game.Gameplay.Controllers
{
    [RequireDataField(typeof(IsRunningField))]
    [RequireDataField(typeof(DirectionField))]
    public class PlayerController : UnitController
    {
        [SerializeField, TabGroup("Components")] private CharacterController _characterController;
        [SerializeField, TabGroup("Parameters")] private float _speedAcceleration;
        [SerializeField, TabGroup("Parameters")] private LayerMask _groundLayer;
        [SerializeField, TabGroup("Parameters")] private MonoBehaviour[] _runningComponent;
        [SerializeField, TabGroup("Animation")] private string _onFallKey = "OnFall";
        
        [ShowInInspector] private float _currentSpeed;

        private bool _isRunning;
        private IsRunningField _isRunningField;
        private DirectionField _directionField;
        
        protected override void Awake()
        {
            base.Awake();
            _isRunningField = GetDataField<IsRunningField>(true);
            _directionField = GetDataField<DirectionField>(true);
        }

        private void Start()
        {
            _directionField.SetValue(Vector3.forward);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _currentSpeed = _speed;
            
            SetMotionAnimation(_isRunningField.Value);
            
            IsRunningFieldChangeHandler(_isRunningField.Value);
            _isRunningField.OnChanged += IsRunningFieldChangeHandler;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _isRunningField.OnChanged -= IsRunningFieldChangeHandler;
        }

        private void IsRunningFieldChangeHandler(bool isRunning)
        {
            if (!isRunning && _isRunning )
            {
                if (_animator != null) _animator.SetTrigger(_onFallKey);
            }

            SetMotionAnimation(isRunning, 1f);
            _isRunning = isRunning;
            
            _runningComponent?.ForEach(component => component.enabled = isRunning);
        }

        public override void Move(Vector3 direction)
        {
            if (direction == Vector3.zero) return;

            var prevPosition = transform.position;
            
            var moveDirection = direction * GetSpeed();
            
            _characterController.Move(moveDirection);

            if (!_characterController.isGrounded)
            {
                var lowestPoint = _characterController.bounds.center;
                lowestPoint.y = _characterController.bounds.min.y;
                if (Physics.Raycast(new Ray(lowestPoint, Vector3.down), out RaycastHit hit, Mathf.Infinity, _groundLayer, QueryTriggerInteraction.Ignore))
                {
                    _characterController.Move(hit.point - transform.position);
                }
            }
            
            RaycastTrigger(prevPosition);
        }

        private void RaycastTrigger(Vector3 direction)
        {
            var position = _characterController.GetExtremePoint(direction);
            if (_characterController.Raycast(new Ray(position, direction), out RaycastHit hit, direction.magnitude))
            {
                if (hit.collider.TryGetComponent(out TriggerZonePlayer triggerZone))
                {
                    triggerZone.Trigger(_characterController);
                }
            }
        }

        public override float GetSpeed()
        {
            return _currentSpeed;
        }

        private void Update()
        {
            if (_isRunningField.Value)
            {
                Move(_directionField.Value * Time.deltaTime);
                _currentSpeed += _speedAcceleration * Time.deltaTime;
                
                SetMotionAnimation(_isRunningField.Value, _currentSpeed / _speed);
            }
        }
    }
}