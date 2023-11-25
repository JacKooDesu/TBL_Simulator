using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.Core.UI
{
    public class UIHandler : MonoBehaviour
    {
        static UIHandler _instance = default;
        public static UIHandler Instance
        {
            get
            {
                if (_instance is null)
                {
                    var obj = new GameObject("UI Handler");
                    _instance = obj.AddComponent<UIHandler>();
                }

                return _instance;
            }
        }

        void Awake()
        {
            if (Instance is null)
            {
                _instance = this;
                return;
            }

            if (Instance != this)
                Destroy(gameObject);
        }

        public Stack<UILayer> _layers;
        
    }
}
