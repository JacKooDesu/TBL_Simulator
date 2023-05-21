using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.Game
{
    [System.Serializable]
    public class Player
    {
        [SerializeField] ProfileStatus profileStatus = new ProfileStatus();
        public ProfileStatus ProfileStatus => profileStatus;

        [SerializeField] TeamStatus teamStatus = new TeamStatus();
        public TeamStatus TeamStatus => teamStatus;

        [SerializeField] CardStatus cardStatus = new CardStatus();
        public CardStatus CardStatus => cardStatus;

        [SerializeField] HeroStatus heroStatus = new HeroStatus();
        public HeroStatus HeroStatus => heroStatus;

        [SerializeField] SkillStatus skillStatus = new SkillStatus();
        public SkillStatus SkillStatus => skillStatus;

        List<IPlayerStatus> StatusList => new List<IPlayerStatus>{
            profileStatus,
            teamStatus,
            cardStatus,
            heroStatus,
            skillStatus
        };

        public Player()
        {
            profileStatus = new ProfileStatus();
        }

        // 更新狀態
        public void UpdateStatus<T>(PlayerStatusType type, T status) where T : IPlayerStatus
        {
            switch (type)
            {
                case PlayerStatusType.ProfileStatus:
                    ProfileStatus.Update(status);
                    break;

                case PlayerStatusType.CardStatus:
                    CardStatus.Update(status);
                    break;

                case PlayerStatusType.Hero:
                    HeroStatus.Update(status);
                    break;

                case PlayerStatusType.Skill:
                    SkillStatus.Update(status);
                    break;

                case PlayerStatusType.TeamStatus:
                    TeamStatus.Update(status);
                    break;

                default:
                    Debug.LogError("Target not found!");
                    break;
            }
        }
    }
}
