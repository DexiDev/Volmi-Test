using Game.Data;
using UnityEngine;

namespace Game.Levels
{
    [CreateAssetMenu(menuName = "Data/Level/Level Config", fileName = "Level Config")]
    public class LevelConfig : DataConfig<LevelData>
    {
        [SerializeField] public int SkipForLoop { get; private set; }
    }
}