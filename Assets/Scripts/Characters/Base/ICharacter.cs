using Game.Assets;
using Game.Data;
using UnityEngine;

namespace Game.Characters
{
    public interface ICharacter : IAsset, IDataController
    {
        Transform Transform { get; }
        
        void Move(Vector3 direction);
        
        void SetRotation(Quaternion targetRotation);
    }
}