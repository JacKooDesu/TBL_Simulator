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
            public IPlayerStatus?[] ToEnum() =>
                new IPlayerStatus?[]{
                    profileStatus,
                    cardStatus,
                    heroStatus,
                    skillStatus,
                    teamStatus
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