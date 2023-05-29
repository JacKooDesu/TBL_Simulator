#nullable enable
using System;
using Newtonsoft.Json;

namespace TBL.Game.Networking
{
    [JsonObject]
    public class PlayerStatusPacket : IPacket<PlayerStatusPacket.StatusData>
    {
        [JsonObject]
        public struct StatusData
        {
            public ProfileStatus? profileStatus;
            public CardStatus? cardStatus;
            public HeroStatus? heroStatus;
            public SkillStatus? skillStatus;
            public TeamStatus? teamStatus;
        }
        public PlayerStatusPacket(StatusData data)
        {
            this.Type = PacketType.PlayerStatus;
            this.Data = data;
        }
        [JsonProperty]
        public StatusData Data { get; private set; }
        [JsonProperty]
        public PacketType Type { get; private set; }

        public bool Serialize(ref string data)
        {
            data = PacketUtil.Serialize(this);
            return true;
        }
    }
}