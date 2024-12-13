using System.Collections.Generic;
using Game.Items;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Inventory
{
    [CreateAssetMenu(menuName = "Data/Inventory/Inventory Config", fileName = "Inventory Config")]
    public class InventoryConfig : SerializedScriptableObject
    {
        [field: SerializeField] public Dictionary<ItemData, int> StartingInventory { get; private set; }
    }
}