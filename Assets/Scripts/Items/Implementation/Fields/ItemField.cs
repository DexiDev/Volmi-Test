using Game.Data.Attributes;
using Game.Data.Fields;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Items.Fields
{
    public class ItemField : IDCountField
    {
        [DataID(typeof(ItemsConfig))]
        [SerializeField, OnValueChanged(nameof(OnValueChanged))] protected new string _value;
        
        public override string Value
        {
            get => _value;
            protected set => _value = value;
        }
    }
}