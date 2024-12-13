using Game.Data.Fields.Follow;
using UnityEngine;

namespace Game.Data.Handlers
{
    public class FollowTargetPointHandler<TDataController> : FollowTargetHandler<FollowTargetPointField, TDataController> where TDataController : MonoBehaviour, IDataController
    {
        protected override Vector3 GetPosition()
        {
            return _followTargetField.Value;
        }
    }
    
    public class FollowTargetPointHandler : FollowTargetPointHandler<DataController>
    {
        
    }
}