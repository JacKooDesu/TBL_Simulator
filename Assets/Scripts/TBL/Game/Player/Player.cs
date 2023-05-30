using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TBL.Game
{
    using Game.Sys;
    using Game.Networking;
    [System.Serializable]
    public class Player
    {
        [SerializeField] IPlayerStandalone playerStandalone;
        // TODO: player shoud use property drawer to optimized!
        [SerializeField] ProfileStatus profileStatus = new ProfileStatus();
        public ProfileStatus ProfileStatus => profileStatus;

        [SerializeField] TeamStatus teamStatus = new TeamStatus(0, PlayerStatusType.Team);
        public TeamStatus TeamStatus => teamStatus;

        [SerializeField] CardStatus cardStatus = new CardStatus();
        public CardStatus CardStatus => cardStatus;

        [SerializeField] HeroStatus heroStatus = new HeroStatus();
        public HeroStatus HeroStatus => heroStatus;

        [SerializeField] SkillStatus skillStatus = new SkillStatus();
        public SkillStatus SkillStatus => skillStatus;
        [SerializeField] ReceiverStatus receiverStatus = new ReceiverStatus(0);
        public ReceiverStatus ReceiverStatus => receiverStatus;

        List<IPlayerStatus> StatusList => new List<IPlayerStatus>{
            profileStatus,
            teamStatus,
            cardStatus,
            heroStatus,
            skillStatus
        };

        public Player(IPlayerStandalone standalone)
        {
            playerStandalone = standalone;
        }

        // 更新狀態
        public void UpdateStatus<S>(S status) where S : IPlayerStatus
        {
            switch (status.Type())
            {
                case PlayerStatusType.Profile:
                    ProfileStatus.Update(status as ProfileStatus);
                    break;

                case PlayerStatusType.Card:
                    CardStatus.Update(status as CardStatus);
                    break;

                case PlayerStatusType.Hero:
                    HeroStatus.Update(status as HeroStatus);
                    break;

                case PlayerStatusType.Skill:
                    SkillStatus.Update(status as SkillStatus);
                    break;

                case PlayerStatusType.Team:
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
                case PlayerStatusType.Profile:
                    ProfileStatus.Update(status as ProfileStatus);
                    break;

                case PlayerStatusType.Card:
                    CardStatus.Update(status as CardStatus);
                    break;

                case PlayerStatusType.Hero:
                    HeroStatus.Update(status as HeroStatus);
                    break;

                case PlayerStatusType.Skill:
                    SkillStatus.Update(status as SkillStatus);
                    break;

                case PlayerStatusType.Team:
                    TeamStatus.Update(status as ValueTypeStatus<TeamEnum>);
                    break;

                default:
                    Debug.LogError("Target not found!");
                    break;
            }
        }

        public void Handler(string data)
        {

        }
    }
}
