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

        public void Clear()
        {
            text.text = "";
        }

        public void Show(string s)
        {
            text.text = s;
        }
    }

}
