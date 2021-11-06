using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TBL.UI.GameScene
{
    public class TipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public bool clickHide = false;
        [TextArea(3, 10)]
        public string content;

        public void OnPointerEnter(PointerEventData eventData)
        {
            print(content);
            TipCanvas.Singleton.Show(content);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TipCanvas.Singleton.Clear();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (clickHide)
                TipCanvas.Singleton.Clear();
        }
    }
}
