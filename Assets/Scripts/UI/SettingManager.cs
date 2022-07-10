using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.UI
{
    public class SettingManager : MonoBehaviour
    {
        [Header("顏色設定")]
        [SerializeField] Color red;
        public static Color RED;
        [SerializeField] Color blue;
        public static Color BLUE;
        [SerializeField] Color gray;
        public static Color GRAY;
        [SerializeField] Color black;
        public static Color BLACK;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            RED = red;
            BLUE = blue;
            GRAY = gray;
            BLACK = black;
        }
    }
}
