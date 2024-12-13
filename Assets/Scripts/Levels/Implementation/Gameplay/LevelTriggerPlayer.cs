using System;
using Game.Assets;
using Game.Gameplay.TriggerZones;
using UnityEngine;

namespace Game.Levels.Gameplay
{
    public abstract class LevelTriggerPlayer : TriggerZonePlayer, IAsset
    {
        [SerializeField] private BoxCollider _boxCollider;
        [SerializeField] private AssetGroupData _assetGroupData;
        
        public BoxCollider BoxCollider => _boxCollider;
        public IAsset Contract { get; set; }
        public event Action<IAsset> OnReleased;
        public GameObject Instance => gameObject;
        public AssetGroupData AssetGroup  => _assetGroupData;
        
        protected override void OnDisable()
        {
            base.OnDisable();
            OnReleased?.Invoke(this);
        }
    }
}