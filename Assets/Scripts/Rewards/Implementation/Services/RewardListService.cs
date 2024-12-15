using System;
using System.Linq;
using Game.Rewards.Data;
using VContainer;

namespace Game.Rewards.Services
{
    public class RewardListService : IRewardService
    {
        public event Action<IRewardField> OnReward;

        private RewardManager _rewardManager;

        [Inject]
        private void Install(RewardManager rewardManager)
        {
            _rewardManager = rewardManager;
        }
        
        public Type GetTypeData()
        {
            return typeof(RewardListField);
        }

        public void Reward(IRewardField rewardField)
        {
            if (rewardField is RewardListField rewardListField)
            {
                rewardListField.Value?.ForEach(rewardItem => _rewardManager.Reward(rewardItem));
                OnReward?.Invoke(rewardListField);
            }
        }

        public IRewardField[] GetRewards(IRewardField rewardField)
        {
            if (rewardField is RewardListField rewardListField)
            {
                var rewardFields = rewardListField.Value?.SelectMany(reward => _rewardManager.GetRewards(reward) ?? Enumerable.Empty<IRewardField>());
                if (rewardFields != null)
                {
                    rewardFields = rewardFields.Where(reward => reward != null);
                    if (rewardFields.Any())
                    {
                        return rewardFields.ToArray();
                    }
                }
            }

            return null;
        }
    }
}