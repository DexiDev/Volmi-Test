using Game.Data.Fields.Follow;
using UnityEngine;

namespace Game.Data.Handlers
{
    public class FollowTargetTransformHandler<TDataController> : FollowTargetHandler<FollowTargetTransformField, TDataController> where TDataController : MonoBehaviour, IDataController
    {
        protected override Vector3 GetPosition()
        {
            return _followTargetField.Value.position;
        }
    }

    public class FollowTargetTransformHandler : FollowTargetTransformHandler<DataController>
    {
        
    }
}