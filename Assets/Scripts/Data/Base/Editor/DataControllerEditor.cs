#if UNITY_EDITOR
using Game.Data.Attributes.Fields;
using UnityEditor;

namespace Game.Data
{
    
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector.Editor;
    [CustomEditor(typeof(DataController), true)]
    public class DataControllerEditor : OdinEditor
#else
    [CustomEditor(typeof(DataController), true)]
    public class DataControllerEditor : Editor
#endif
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (target is DataController dataController)
            {
                RequireDataFieldAttributeDrawer.OnInspectorGUI(dataController);
                UniqueDataFieldAttributeDrawer.OnInspectorGUI(dataController);
            }
        }
    }
}

#endif