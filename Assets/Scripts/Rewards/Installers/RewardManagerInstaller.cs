using VContainer;
using VContainer.Unity;

namespace Game.Rewards.Installers
{
    public class RewardManagerInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<RewardManager>().AsSelf();
        }
    }
}