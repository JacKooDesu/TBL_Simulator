using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JacDev.Utils.TooltipSystem
{
    public class TooltipSystem : MonoBehaviour
    {
        static TooltipSystem current;

        public Tooltip tooltip;

        private void Awake()
        {
            current = this;
        }

        public static void Show(string content, string header = "")
        {
            current.tooltip.SetText(content, header);
            current.tooltip.gameObject.SetActive(true);
        }

        public static void Hide()
        {
            current.tooltip.gameObject.SetActive(false);
        }
    }
}
