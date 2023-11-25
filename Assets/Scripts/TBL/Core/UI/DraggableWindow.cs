using UnityEngine;

namespace TBL.Core.UI
{
    [RequireComponent(typeof(Window))]
    public class DraggableWindow : MonoBehaviour
    {
        public UIStacker.Index group;
    }
}