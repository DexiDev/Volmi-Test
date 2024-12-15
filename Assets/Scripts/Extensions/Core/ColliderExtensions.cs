using UnityEngine;

namespace Game.Extensions.Core
{
    public static class ColliderExtensions
    {
        public static Vector3 GetExtremePoint(this Collider collider, Vector3 direction)
        {
            direction = direction.normalized;
            Bounds bounds = collider.bounds;
            Vector3 extremePoint = bounds.center;
            Vector3 extents = bounds.extents;
            
            extremePoint += new Vector3(
                direction.x * extents.x, 
                direction.y * extents.y, 
                direction.z * extents.z
            );

            return extremePoint;
        }
    }
}