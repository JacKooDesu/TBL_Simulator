using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.Game.Sys
{

    [CreateAssetMenu(fileName = "TeamAsset", menuName = "TBL/Asset/Team", order = 0)]
    public class TeamAsset : ScriptableObject
    {

        [System.Serializable]
        public class Setting
        {
            public TeamEnum id;
            public Sprite icon;
        }

        public List<Setting> Settings => settings;
        [SerializeField] List<Setting> settings;

        public Setting Get(int id) => Settings.FirstOrDefault(x => ((int)x.id) == id);
        public Setting Get(TeamEnum id) => Settings.FirstOrDefault(x => x.id == id);
    }
}