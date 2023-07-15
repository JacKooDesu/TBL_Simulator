#nullable enable
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TBL.Game.Networking
{
    // [JsonObject]
    public class PlayerStatusPacket : IPacket
    {
        // [JsonObject]
        public struct StatusData
        {
            public ProfileStatus? profileStatus;
            public CardStatus? cardStatus;
            public HeroStatus? heroStatus;
            public SkillStatus? skillStatus;
            public TeamStatus? teamStatus;
            public PhaseQuestStatus? phaseQuestStatus;
            public ReceiverStatus? receiverStatus;
            public IPlayerStatus?[] ToArray() =>
                new IPlayerStatus?[]{
                    profileStatus,
                    cardStatus,
                    heroStatus,
                    skillStatus,
                    teamStatus,
                    phaseQuestStatus,
                    receiverStatus
                };
        }
        [JsonConstructor]
        public PlayerStatusPacket(StatusData data)
        {
            this.Data = data;
        }
        public PlayerStatusPacket(params IPlayerStatus[] statuses)
        {
            StatusData data = new();
            foreach (var s in statuses)
            {
                switch (s.Type())
                {
                    case PlayerStatusType.Team:
                        data.teamStatus = s as TeamStatus;
                        continue;

                    case PlayerStatusType.Card:
                        data.cardStatus = s as CardStatus;
                        continue;

                    case PlayerStatusType.Hero:
                        data.heroStatus = s as HeroStatus;
                        continue;

                    case PlayerStatusType.Skill:
                        data.skillStatus = s as SkillStatus;
                        continue;

                    case PlayerStatusType.Profile:
                        data.profileStatus = s as ProfileStatus;
                        continue;

                    case PlayerStatusType.Quest:
                        data.phaseQuestStatus = s as PhaseQuestStatus;
                        continue;

                    case PlayerStatusType.Reciver:
                        data.receiverStatus = s as ReceiverStatus;
                        continue;
                }
            }
            this.Data = data;
        }
        public PlayerStatusPacket(Player player)
        {
            this.Data = new StatusData
            {
                profileStatus = player.ProfileStatus,
                cardStatus = player.CardStatus,
                heroStatus = player.HeroStatus,
                skillStatus = player.SkillStatus,
                teamStatus = player.TeamStatus,
                phaseQuestStatus = player.PhaseQuestStatus,
                receiverStatus = player.ReceiverStatus,
            };
        }
        [JsonProperty]
        public StatusData Data { get; private set; }

        public PacketType Type() => PacketType.PlayerStatus;

        public bool Serialize(ref string data)
        {
            data = PacketUtil.Serialize(this);
            return true;
        }
    }
}