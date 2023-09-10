using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace TBL.Game.Sys
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "HeroSkillAsset", menuName = "TBL/Asset/HeroSkillAsset", order = 0)]
    public class HeroSkillAsset : ScriptableObject
    {
        [field: SerializeField]
        public int ID { get; private set; }
        [field: SerializeField]
        public string Name { get; private set; }
        [field: SerializeField, TextArea(2, 10)]
        public string Desc { get; private set; }

#if UNITY_EDITOR
        const string dir = "Assets/Settings/Asset/HeroSkill/";
        public static HeroSkillAsset Create(int id, string heroName, string skillName)
        {
            var path = dir + $"{heroName}/";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            HeroSkillAsset result = null;
            path += $"{skillName}.asset";
            if (!File.Exists(path))
            {
                result = ScriptableObject.CreateInstance<HeroSkillAsset>();
                AssetDatabase.CreateAsset(result, path);
                AssetDatabase.SaveAssets();
            }
            else
                result = AssetDatabase.LoadAssetAtPath<HeroSkillAsset>(path);

            result.ID = id;

            return result;
        }
#endif
    }
}
