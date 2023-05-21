using UnityEngine;
using System.Collections.Generic;
using TBL;

namespace TBL.Settings
{
    using Game.Hero;
    [CreateAssetMenu(fileName = "HeroList", menuName = "TBL/Settings/Hero List", order = 0)]
    public class HeroList : ScriptableObject
    {
        public List<HeroBase> heros;
        public Sprite hiddenAvatar;
    }
}
