using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.Game.Sys
{
    using Hero;
    [CreateAssetMenu(fileName = "HeroAsset", menuName = "TBL/Asset/Hero", order = 0)]
    public class HeroAsset : ScriptableObject
    {
        [System.Serializable]
        public class Setting
        {
            public HeroId id;
            public Sprite icon;
        }

        public List<Setting> Settings => settings;
        [SerializeField] List<Setting> settings;

        public Setting Get(int id) => Settings.FirstOrDefault(x => ((int)x.id) == id);
        public Setting Get(HeroId id) => Settings.FirstOrDefault(x => x.id == id);
    }
}