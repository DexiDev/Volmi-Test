using System.Collections.Generic;
using Game.Data;
using UnityEngine;

namespace Game.Items
{
    [CreateAssetMenu(menuName = "Data/Items/Item Config", fileName = "Item Config")]
    public class ItemsConfig : DataConfig<ItemData>
    {
        [field: SerializeField] public Dictionary<ItemData, int> StartingItems { get; private set; }
    }
}