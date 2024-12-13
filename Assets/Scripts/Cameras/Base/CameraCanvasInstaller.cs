using UnityEngine;

namespace Game.Cameras
{
    [DisallowMultipleComponent]
    public class CameraCanvasInstaller : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        
        private Canvas _canvas;

        private void Awake()
        {
            if (_camera == null) _camera = Camera.main;
            
            _canvas = GetComponent<Canvas>();
        }

        private void OnEnable()
        {
            _canvas.worldCamera = _camera;
        }
    }
}