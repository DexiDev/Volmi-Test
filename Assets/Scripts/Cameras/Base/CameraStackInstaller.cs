using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Game.Cameras
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class CameraStackInstaller : MonoBehaviour
    {
        private void Awake()
        {
            if (TryGetComponent(out Camera camera))
            {
                var mainCamera = Camera.main;
                
                if (!camera.Equals(mainCamera))
                {
                    var cameraData = mainCamera.GetUniversalAdditionalCameraData();
                    cameraData.cameraStack.Add(camera);
                }
            }
        }
    }
}