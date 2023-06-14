#nullable enable
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TBL.Game.Networking
{
    using Game.Sys;
    [JsonObject]
    public class GameStartPacket : IPacket
    {
        [JsonProperty]
        public Unit _ { get; private set; }
        public PacketType Type() => PacketType.GameStart;

        public GameStartPacket()
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