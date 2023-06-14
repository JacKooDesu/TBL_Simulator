#nullable enable
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TBL.Game.Networking
{
    using Game.Sys;
    [JsonObject]
    public class PlayerReadyPacket : IPacket
    {
        [JsonProperty]
        public Unit _ { get; private set; }
        public PacketType Type() => PacketType.PlayerReady;

        public PlayerReadyPacket()
        {
            _ = new();
        }

        public bool Serialize(ref string data)
        {
            data = PacketUtil.Serialize(this);
            return true;
        }
    }
}