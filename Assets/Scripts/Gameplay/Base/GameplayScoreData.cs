using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Gameplay
{
    public class GameplayScoreData
    {
        private string _levelID;
        private Dictionary<string, int> _items;
        
        public string LevelID => _levelID;
        public Dictionary<string, int> Items => _items;
        
        public event Action<string, int> OnItemChanged;

        public GameplayScoreData(string levelID)
        {
            _levelID = levelID;
            _items = new();
        }

        public void AddItem(string itemID, int count)
        {
            if (count < 1) return;
            
            if (!_items.TryAdd(itemID, count))
            {
                _items[itemID] += count;
            }
            
            OnItemChanged?.Invoke(itemID, count);
        }

        public int GetItemsAllCount()
        {
            return _items?.Values.Sum() ?? 0;
        }
    }
}