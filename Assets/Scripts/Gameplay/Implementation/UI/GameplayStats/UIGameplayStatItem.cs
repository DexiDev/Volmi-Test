using Game.Items;
using Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace Game.Gameplay.UI.GameplayStats
{
    public class UIGameplayStatItem : UIElement
    {
        [SerializeField] private TMP_Text _textField;

        private ItemsManager _itemsManager;

        [Inject]
        private void Install(ItemsManager itemsManager)
        {
            _itemsManager = itemsManager;
        }
        
        public void Initialize(string itemID, int value)
        {
            var itemData = _itemsManager.GetData(itemID);

            if (itemData != null)
            {
                _textField.text = $"{itemData.Name}: {value}";
            }
            else gameObject.SetActive(false);
        }
    }
}