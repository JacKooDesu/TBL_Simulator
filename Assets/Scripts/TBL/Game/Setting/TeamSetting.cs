using UnityEngine;
using System.Collections.Generic;
using System;

namespace TBL.Game.Setting
{
    [CreateAssetMenu(fileName = "TeamSetting", menuName = "TBL/Game/TeamSetting", order = 0)]
    public class TeamSetting : ScriptableObject
    {
        [Serializable]
        public struct Set
        {
            public int PlayerCount;

            public int Blue;
            public int Red;
            public int Green;
        }

        public List<Set> PlayerSets = new List<Set>();

        [ContextMenu("Init Defaults")]
        void InitDefaults()
        {
            PlayerSets.Clear();

            PlayerSets = new List<Set>(){
                new Set{
                    PlayerCount=2,
                    Blue=1,
                    Red=1,
                    Green=0
                },
                new Set{
                    PlayerCount=3,
                    Blue=1,
                    Red=1,
                    Green=0
                },
                new Set{
                    PlayerCount=4,
                    Blue=2,
                    Red=2,
                    Green=0
                },
                new Set{
                    PlayerCount=5,
                    Blue=2,
                    Red=2,
                    Green=1
                },
                new Set{
                    PlayerCount=6,
                    Blue=2,
                    Red=2,
                    Green=2
                },
                new Set{
                    PlayerCount=7,
                    Blue=2,
                    Red=2,
                    Green=3
                },
                new Set{
                    PlayerCount=8,
                    Blue=3,
                    Red=3,
                    Green=2
                }
            };
        }
    }
}