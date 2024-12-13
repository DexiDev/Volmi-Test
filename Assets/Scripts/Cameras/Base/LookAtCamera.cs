using UnityEngine;

namespace Game.Cameras
{
    [DisallowMultipleComponent]
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        private void Awake()
        {
            if(_camera == null)  _camera = Camera.main;
        }

        private void LateUpdate()
        {
            transform.LookAt(_camera.transform);
        }
    }
}