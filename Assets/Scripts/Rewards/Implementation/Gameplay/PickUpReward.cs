using Game.Gameplay.Controllers;
using Game.Zones;
using UnityEngine;
using VContainer;

namespace Game.Rewards.Gameplay
{
    public class PickUpReward : TriggerZoneCollider<PlayerController, BoxCollider>
    {
        [SerializeField] private IRewardField _rewardField;
        
        private RewardManager _rewardManager;
        
        [Inject]
        private void Install(RewardManager rewardManager)
        {
            _rewardManager = rewardManager;
        }

        protected override void Trigger(PlayerController controller)
        {
            base.Trigger(controller);
            
            _rewardManager.Reward(_rewardField);
            
            gameObject.SetActive(false);
        }
    }
}