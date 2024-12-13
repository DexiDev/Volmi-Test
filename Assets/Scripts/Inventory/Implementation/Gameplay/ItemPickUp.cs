using Game.Data.Attributes;
using Game.Gameplay.Controllers;
using Game.Items;
using Game.Levels.Gameplay;
using UnityEngine;
using VContainer;

namespace Game.Inventory.Gameplay
{
    public class ItemPickUp : LevelTriggerPlayer
    {
        [SerializeField, DataID(typeof(ItemsConfig))] private string _itemID;
        [SerializeField] private int _count = 1;
        
        private InventoryManager _inventoryManager;
        
        [Inject]
        private void Install(InventoryManager inventoryManager)
        {
            _inventoryManager = inventoryManager;
        }

        protected override void TriggerAction(PlayerController controller)
        {
            base.TriggerAction(controller);
            
            _inventoryManager.Add(_itemID, _count);
            
            gameObject.SetActive(false);
        }
    }
}