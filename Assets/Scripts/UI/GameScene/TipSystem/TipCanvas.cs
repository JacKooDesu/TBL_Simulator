using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TBL.UI.GameScene
{
    public class TipCanvas : MonoBehaviour
    {
        static TipCanvas singleton = null;
        public static TipCanvas Singleton
        {
            get
            {
                if (singleton != null)
                    return singleton;

                singleton = FindObjectOfType<TipCanvas>();

                if (singleton == null)
                {
                    GameObject g = new GameObject("TipCanvas");
                    singleton = g.AddComponent<TipCanvas>();
                }

                return singleton;
            }
        }

        public Text text;

        public string priorityMessage = "";
        public void ResetPriorityMessage() { priorityMessage = ""; }

        public void Clear()
        {
            text.text = "";
        }

        public void Show(string s, bool highPriority = false)
        {
            if (highPriority)
            {
                priorityMessage = s;
                text.text = priorityMessage;
            }

            if (priorityMessage.Length != 0)
                return;

            text.text = s;
        }
    }

}
