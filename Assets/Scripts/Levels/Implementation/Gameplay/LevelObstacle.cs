using System;
using Game.Assets;
using Game.Gameplay;
using Game.Gameplay.Controllers;
using Game.Gameplay.TriggerZones;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace Game.Levels.Gameplay
{
    public class LevelObstacle : LevelTriggerPlayer
    {
        private GameplayManager _gameplayManager;
        
        [Inject]
        private void Install(GameplayManager gameplayManager)
        {
            _gameplayManager = gameplayManager;
        }

        protected override void TriggerAction(PlayerController controller)
        {
            base.TriggerAction(controller);
            
            _gameplayManager.GameplayController.StopGame();
        }
    }
}