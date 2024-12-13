using System;
using System.Collections.Generic;
using System.Linq;
using Game.Items;
using Game.Levels;
using Game.Save;
using Sirenix.Utilities;
using VContainer;
using VContainer.Unity;

namespace Game.Inventory
{
    public class InventoryManager : IInitializable
    {
        private const string _saveID = nameof(InventoryManager);
        private const string _saveIDStart = _saveID + ".IsSetStart";

        private InventoryConfig _config;
        
        private ItemsManager _itemsManager;
        private SaveManager _saveManager;
        
        private Dictionary<string, int> _items = new();
        
        public event Action<string, int> OnItemChanged;
        public event Action<string, int> OnItemAdded;
        public event Action<string, int> OnItemRemoved;
        
        [Inject]
        private void Install(InventoryConfig inventoryConfig, ItemsManager itemsManager, SaveManager saveManager)
        {
            _config = inventoryConfig;
            _itemsManager = itemsManager;
            _saveManager = saveManager;
        }

        
        public void Initialize()
        {
            if (!_saveManager.TryGetData<bool>(_saveIDStart, out var resultIsStart))
            {
                _config.StartingInventory?.ForEach(item => Add(item.Key.ID, item.Value));
                _saveManager.SetData(_saveIDStart, true);
            }
            
            if (_saveManager.TryGetData<Dictionary<string, int>>(_saveID, out var result))
                _items = result;
        }

        public void Add(string itemID, int count)
        {
            var itemData = _itemsManager.GetData(itemID);

            if (itemData != null)
            {
                if (!_items.ContainsKey(itemID)) _items.Add(itemID, 0);

                var resultCount = _items[itemID] + count;

                if (itemData.StackLimit != -1)
                {
                    if (resultCount > itemData.StackLimit)
                    {
                        resultCount = itemData.StackLimit;
                    }
                    else resultCount = Math.Clamp(resultCount, resultCount, itemData.StackLimit);
                }

                var oldCount = _items[itemID]; 
                
                _items[itemID] = resultCount;
                
                _saveManager.SetData(_saveID, _items);
                
                OnItemChanged?.Invoke(itemID, resultCount);
                OnItemAdded?.Invoke(itemID, resultCount - oldCount);
            }
        }

        public void Remove(string itemID, int count)
        {
            if (_items.ContainsKey(itemID))
            {
                var sourceCount = _items[itemID];
                
                var resultCount = sourceCount - count;
                resultCount = Math.Clamp(resultCount, 0, resultCount);
                    
                _items[itemID] = resultCount;

                _saveManager.SetData(_saveID, _items);
                
                OnItemChanged?.Invoke(itemID, resultCount);
                OnItemRemoved?.Invoke(itemID, resultCount - sourceCount);
            }
        }

        public bool ContainsItem(string targetItem, int count = 1)
        {
            return GetItemCount(targetItem) >= count;
        }

        public int GetItemCount(string targetItem)
        {
            return _items.GetValueOrDefault(targetItem, 0);
        }

        public int GetItemAllCount()
        {
            var items = _items.ToArray();

            int count = 0;

            items?.ForEach(item => count += item.Value);

            return count;
        }
    }
}