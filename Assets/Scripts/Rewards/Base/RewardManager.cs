using System;
using System.Collections.Generic;
using System.Linq;
using Game.Inventory.Rewards;
using Game.Rewards.Services;
using Sirenix.Utilities;
using VContainer;
using VContainer.Unity;

namespace Game.Rewards
{
    public class RewardManager : IInitializable
    {
        private static readonly IRewardService[] _serviceInstance = {
            new ItemRewardService(),
            new RewardListService(),
        };

        private Dictionary<Type, IRewardService> _rewardServices = new();

        private IObjectResolver _objectResolver;
        
        private bool _isInitialize;
        
        public event Action<IRewardField> OnReward;

        [Inject]
        private void Install(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public void Initialize()
        {
            if (_isInitialize) return;
            
            _isInitialize = true;
            
            _serviceInstance.ForEach(_objectResolver.Inject);
            
            _rewardServices = _serviceInstance.ToDictionary(key => key.GetTypeData(), value => value);

            _serviceInstance.ForEach(service => service.OnReward += OnReward);
        }

        public void Reward(IRewardField rewardField)
        {
            if (rewardField == null) return;
            
            if(!_isInitialize) Initialize();
            
            if (_rewardServices.TryGetValue(rewardField.GetType(), out var service))
            {
                service.Reward(rewardField);
            }
        }

        public IRewardField[] GetRewards(IRewardField rewardField)
        {
            if (rewardField != null)
            {
                if (!_isInitialize) Initialize();

                if (_rewardServices.TryGetValue(rewardField.GetType(), out var service))
                {
                    return service.GetRewards(rewardField);
                }
            }

            return null;
        }
    }
}