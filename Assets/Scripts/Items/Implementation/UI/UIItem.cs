using Game.Data.Attributes;
using Game.Items;
using Game.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Inventory.UI
{
    public class UIItem : UIElement
    {
        private enum CountType
        {
            Global,
            AfterStart,
            Static
        }
        
        [SerializeField, DataID(typeof(ItemsConfig))] private string _itemID;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _nameField;
        [SerializeField] private TMP_Text _textField;
        [SerializeField] private string _suffixName = ":"; 
        [SerializeField] private CountType _countType;
        
        private int _count;
        private ItemsManager _itemsManager;

        [Inject]
        private void Install(ItemsManager itemsManager)
        {
            _itemsManager = itemsManager;
        }

        public void Initialize(string itemID, int count = 0)
        {
            SetItemID(itemID, count);
        }

        private void OnEnable()
        {
            SetItemID(_itemID);

            if (_countType is CountType.Global or CountType.AfterStart)
            {
                _itemsManager.OnItemAdded += OnItemChanged;
                _itemsManager.OnItemRemoved += OnItemChanged;
            }
        }

        private void SetItemID(string itemID, int count = 0)
        {
            _itemID = itemID;
            
            var itemData = _itemsManager.GetData(itemID);
            if (itemData != null)
            {
                SetName(itemData);
                SetIcon(itemData);
            }
            
            _count = _countType == CountType.Global ? _itemsManager.GetItemCount(_itemID) : count;
            
            OnItemChanged(_itemID, count);
        }

        private void SetName(ItemData itemData)
        {
            if (_nameField == null || itemData == null) return;
            
            _nameField.text = itemData.Name + _suffixName;
        }

        private void SetIcon(ItemData itemData)
        {
            if (_icon == null || itemData == null) return;
            
            _icon.sprite = itemData.Icon;
            _icon.color = itemData.IconColor;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_countType is CountType.Global or CountType.AfterStart)
            {
                _itemsManager.OnItemAdded -= OnItemChanged;
                _itemsManager.OnItemRemoved -= OnItemChanged;
            }
        }

        private void OnItemChanged(string item, int count)
        {
            if (item != _itemID) return;

            switch (_countType)
            {
                case CountType.Global:
                    _count = _itemsManager.GetItemCount(item);
                    break;
                case CountType.AfterStart:
                    _count += count;
                    break;
                case CountType.Static:
                    _count = count;
                    break;
            }

            _textField.text = $"{_count}";
        }
    }
}