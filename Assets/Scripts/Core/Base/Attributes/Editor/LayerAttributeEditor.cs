using UnityEditor;
using UnityEngine;

namespace Game.Core.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    class LayerAttributeEditor : PropertyDrawer
    {
  
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.LayerField(position, label,  property.intValue);
        }

    }
}