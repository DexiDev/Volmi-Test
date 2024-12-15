using Game.Gameplay;
using Game.Gameplay.Controllers;
using Game.Zones;
using UnityEngine;
using VContainer;

namespace Game.Levels.Gameplay
{
    public class LevelObstacleTrigger : TriggerZoneCollider<PlayerController, BoxCollider>
    {
        private GameplayManager _gameplayManager;
        
        [Inject]
        private void Install(GameplayManager gameplayManager)
        {
            _gameplayManager = gameplayManager;
        }

        protected override void Trigger(PlayerController controller)
        {
            base.Trigger(controller);
            
            _gameplayManager.GameplayController.StopGame();
        }
    }
}