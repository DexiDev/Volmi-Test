using System;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Assets
{
    public interface IAsset
    {
        AssetGroupData AssetGroup { get; }
        
        IAsset Contract { get; set; }
        
        GameObject Instance { get; }
        
        event Action<IAsset> OnReleased;
    }
}