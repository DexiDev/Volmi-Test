using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    public class UIScreen : UIElement
    {
        [SerializeField] protected RectTransform _root;
    }
}