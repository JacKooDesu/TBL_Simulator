using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.Game
{
    public class Player
    {
        public HeroStatus HeroStatus { get; private set; }
        public SkillStatus SkillStatus { get; private set; }
        
        public void UpdateStatus(PlayerStatusType type)
        {
            switch (type)
            {
                case PlayerStatusType.Hero:
                    break;

                case PlayerStatusType.Skill:
                    break;
            }
        }
    }
}
