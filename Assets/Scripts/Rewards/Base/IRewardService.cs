using System;

namespace Game.Rewards
{
    public interface IRewardService
    {
        public event Action<IRewardField> OnReward;
        
        public Type GetTypeData();
        
        public void Reward(IRewardField rewardField);

        public IRewardField[] GetRewards(IRewardField rewardField);
    }
}