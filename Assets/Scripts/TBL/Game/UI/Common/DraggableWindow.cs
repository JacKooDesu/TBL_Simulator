using UnityEngine;

namespace TBL.Game.UI
{
    [RequireComponent(typeof(Window))]
    public class DraggableWindow : MonoBehaviour
    {
        public UIStacker.Index group;
    }
}