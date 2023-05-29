using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TBL.Game
{
    [System.Serializable]
    public class Player
    {
        // TODO: player shoud use property drawer to optimized!
        [SerializeField] ProfileStatus profileStatus = new ProfileStatus();
        public ProfileStatus ProfileStatus => profileStatus;

        [SerializeField] TeamStatus teamStatus = new TeamStatus(TeamEnum.Blue, PlayerStatusType.TeamStatus);
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
        public void UpdateStatus<S>(S status) where S : IPlayerStatus
        {
            switch (status.Type())
            {
                case PlayerStatusType.ProfileStatus:
                    ProfileStatus.Update(status as ProfileStatus);
                    break;

                case PlayerStatusType.CardStatus:
                    CardStatus.Update(status as CardStatus);
                    break;

                case PlayerStatusType.Hero:
                    HeroStatus.Update(status as HeroStatus);
                    break;

                case PlayerStatusType.Skill:
                    SkillStatus.Update(status as SkillStatus);
                    break;

                case PlayerStatusType.TeamStatus:
                    TeamStatus.Update(status as ValueTypeStatus<TeamEnum>);
                    break;

                default:
                    Debug.LogError("Target not found!");
                    break;
            }
        }

        public void UpdateStatus<T>(PlayerStatusType type, T status)
        {
            switch (type)
            {
                case PlayerStatusType.ProfileStatus:
                    ProfileStatus.Update(status as ProfileStatus);
                    break;

                case PlayerStatusType.CardStatus:
                    CardStatus.Update(status as CardStatus);
                    break;

                case PlayerStatusType.Hero:
                    HeroStatus.Update(status as HeroStatus);
                    break;

                case PlayerStatusType.Skill:
                    SkillStatus.Update(status as SkillStatus);
                    break;

                case PlayerStatusType.TeamStatus:
                    TeamStatus.Update(status as ValueTypeStatus<TeamEnum>);
                    break;

                default:
                    Debug.LogError("Target not found!");
                    break;
            }
        }
    }
}
