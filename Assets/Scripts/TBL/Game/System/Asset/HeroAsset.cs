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
#if UNITY_EDITOR
            [HideInInspector] public string DisplayName;
#endif
            public HeroId id;
            public string name;
            public Sprite icon;
        }


        [field: SerializeField]
        public List<Setting> Settings { get; private set; } = new();
        [field: SerializeField]
        public List<HeroSkillAsset> Skills { get; private set; } = new();

        public Setting Get(int id) => Settings.FirstOrDefault(x => ((int)x.id) == id);
        public Setting Get(HeroId id) => Settings.FirstOrDefault(x => x.id == id);

#if UNITY_EDITOR
        [ContextMenu("Auto Build")]
        void AutoBuild()
        {
            var all =
                (Enum.GetValues(typeof(HeroId)) as HeroId[])
                    .Select(x => HeroList.GetHero(x))
                    .Where(x => x is not null);
            foreach (var hero in all)
            {
                var heroName = hero.GetType().Name;
                var setting = Settings.Find(x => x.id == hero.Id);
                if (setting is null)
                    Settings.Add(new()
                    {
                        DisplayName = heroName,
                        id = hero.Id
                    });
                else
                    setting.DisplayName = heroName;

                Skills.Clear();
                Skills.AddRange(
                    hero.Skills()
                        .Select(x =>
                            HeroSkillAsset.Create(
                                x.Id, heroName, x.GetType().Name)));
            }
        }
#endif
    }
}