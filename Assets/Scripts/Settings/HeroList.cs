using UnityEngine;
using System.Collections.Generic;
using TBL;

namespace TBL.Settings
{
    [CreateAssetMenu(fileName = "HeroList", menuName = "TBL/Settings/Hero List", order = 0)]
    public class HeroList : ScriptableObject
    {
        public List<Hero> heros;
        public Sprite hiddenAvatar;
    }
}
