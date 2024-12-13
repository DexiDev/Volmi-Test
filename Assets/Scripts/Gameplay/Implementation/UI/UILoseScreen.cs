using System;
using Game.Levels;
using Game.Loadings;
using Game.UI;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.UI
{
    public class UILoseScreen : UIScreen
    {
        [SerializeField] UIButton _menuButton;

        private LoadingManager _loadingManager;

        [Inject]
        private void Install(LoadingManager loadingManager)
        {
            _loadingManager = loadingManager;
        }
        
        private void OnEnable()
        {
            _menuButton.OnClick += MenuClickHandle;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _menuButton.OnClick -= MenuClickHandle;
        }

        private void MenuClickHandle()
        {
            _loadingManager.LoadMenu();
        }
    }
}