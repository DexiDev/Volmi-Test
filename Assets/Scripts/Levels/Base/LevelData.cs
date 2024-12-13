using Game.Data;
using Game.Data.Attributes;
using Game.Inventory.Gameplay;
using Game.Items.Fields;
using Game.Levels.Gameplay;
using Game.Loadings;
using UnityEngine;

namespace Game.Levels
{
    [CreateAssetMenu(menuName = "Data/Level/Level Data", fileName = "Level Data")]
    public class LevelData : DataScriptable
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, DataID(typeof(LoadingConfig))] public string LoadingID { get; private set; }
        [field: SerializeField] public LevelNode[] NodesContract { get; private set; }
        [field: SerializeField] public ItemPickUp[] ItemRewards { get; private set; }
        [field: SerializeField, Range(0, 100)] public int ChanceReward { get; private set; } = 30;
        [field: SerializeField] public Vector2 MinMaxRewardSpacing { get; private set; } = new Vector2(2f, 3f);
        [field: SerializeField] public LevelObstacle[] ObstaclesContract { get; private set; }
        [field: SerializeField] public Vector2 MinMaxObstacleSpacing { get; private set; } = new Vector2(3f, 10f);
    }
}