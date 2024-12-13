using Game.Data.Handlers;
using UnityEngine;

namespace Game.UI.Handlers
{
    public class UIFollowTargetTransformHandler : FollowTargetTransformHandler<UIElement>
    {
        [SerializeField] private Camera _camera;

        private void Awake()
        {
            if(_camera == null) _camera = Camera.main;
        }
        
        protected override Vector3 GetPosition()
        {
            var position = base.GetPosition();

            return _camera.WorldToScreenPoint(position);
        }
    }
}