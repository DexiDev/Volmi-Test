using System;
using UnityEngine;

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