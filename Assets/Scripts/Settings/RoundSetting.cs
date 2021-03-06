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
        public float heroReactionTime;  // 英雄技能發動等待時間
        public float drawTime;  // 抽牌時間
    }
}
