using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Save
{
    [CreateAssetMenu(menuName = "Data/Save/Save Config", fileName = "Save Config")]
    public class SaveConfig : SerializedScriptableObject
    {
        [field: SerializeField, ReadOnly] public string SaveToken { get; private set; }
        
        #if UNITY_EDITOR
        [Button]
        private void UpdateToken()
        {
            SaveToken = System.Guid.NewGuid().ToString();
        }
        #endif
    }
}