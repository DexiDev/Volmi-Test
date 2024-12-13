using Game.Levels;
using UnityEngine;
using VContainer;

namespace Game.Loadings.Boot
{
    public class BootLoader : MonoBehaviour
    {
        [SerializeField] private int _targetFrameRate = 60;
        
        private LoadingManager _loadingManager;
        
        [Inject]
        private void Install(LoadingManager loadingManager)
        {
            _loadingManager = loadingManager;
        }

        private void Awake()
        {
            Application.targetFrameRate = _targetFrameRate;
        }

        private void Start()
        {
            _loadingManager.LoadMenu();
        }
    }
}