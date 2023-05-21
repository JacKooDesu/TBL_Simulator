using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.Game
{
    [System.Serializable]
    public class Player
    {
        public ProfileStatus ProfileStatus { get; private set; }
        public TeamStatus TeamStatus { get; private set; }
        public CardStatus CardStatus { get; private set; }
        public HeroStatus HeroStatus { get; private set; }
        public SkillStatus SkillStatus { get; private set; }

        public Player()
        {
            
        }

        // 更新狀態
        public void UpdateStatus<T>(PlayerStatusType type, T status) where T : IPlayerStatus
        {
            IPlayerStatus target = null;
            switch (type)
            {
                case PlayerStatusType.ProfileStatus:
                    target = ProfileStatus;
                    break;

                case PlayerStatusType.TeamStatus:
                    target = TeamStatus;
                    break;

                case PlayerStatusType.CardStatus:
                    target = CardStatus;
                    break;

                case PlayerStatusType.Hero:
                    target = HeroStatus;
                    break;

                case PlayerStatusType.Skill:
                    target = SkillStatus;
                    break;

                default:
                    Debug.LogError("Target not found!");
                    break;
            }

            target?.Update(status);
        }
    }
}
