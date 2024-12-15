using System;
using Game.Items;
using Game.Items.Fields;
using Game.Rewards;
using VContainer;

namespace Game.Inventory.Rewards
{
    public class ItemRewardService : IRewardService
    {
        public event Action<IRewardField> OnReward;
        
        private ItemsManager _itemsManager;
        
        [Inject]
        private void Install(ItemsManager itemsManager)
        {
            _itemsManager = itemsManager;
        }
        
        public Type GetTypeData()
        {
            return typeof(ItemField);
        }

        public void Reward(IRewardField rewardField)
        {
            if (rewardField is ItemField itemField)
            {
                _itemsManager.Add(itemField.Value, itemField.Count);
                OnReward?.Invoke(rewardField);
            }
        }

        public IRewardField[] GetRewards(IRewardField rewardField)
        {
            if (rewardField is ItemField itemField)
            {
                return new IRewardField[] { itemField };
            }

            return null;
        }
    }
}