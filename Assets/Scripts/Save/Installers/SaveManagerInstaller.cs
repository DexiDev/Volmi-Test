using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Save.Installers
{
    public class SaveManagerInstaller : IInstaller
    {
        [SerializeField] private SaveConfig _config;

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<SaveManager>().WithParameter(_config).AsSelf();
        }
    }
}