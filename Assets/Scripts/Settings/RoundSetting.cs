using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.Settings
{
    [CreateAssetMenu(fileName = "Round Setting", menuName = "TBL/Settings/RoundSetting", order = 0)]
    public class RoundSetting : ScriptableObject
    {
        public float roundTime;     // 輪操作時間
        public float reactionTime;  // 反制發動時間
    }
}
