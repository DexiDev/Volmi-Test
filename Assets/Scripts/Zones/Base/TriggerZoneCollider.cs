using UnityEngine;

namespace Game.Zones
{
    [RequireComponent(typeof(Collider))]
    public class TriggerZoneCollider<TController, TCollider> : TriggerZone<TController> where TCollider : Collider 
    {
        [SerializeField] private TCollider _collider;
        
        public TCollider Collider => _collider;
    }
}