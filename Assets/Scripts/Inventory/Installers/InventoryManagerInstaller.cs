using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Inventory.Installers
{
    public class InventoryManagerInstaller : IInstaller
    {
        [SerializeField] private InventoryConfig _config;

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<InventoryManager>().WithParameter(_config).AsSelf();
        }
    }
}