using System;
using Newtonsoft.Json;
namespace TBL.Game.Networking
{
    using Game.Sys;
    [JsonObject]
    public class ServerReadyPacket : IPacket
    {
        [JsonProperty]
        public Unit _ { get; private set; }
        public PacketType Type() => PacketType.ServerReady;

        public ServerReadyPacket() => _ = new();

        public bool Serialize(ref string data)
        {
            data = PacketUtil.Serialize(this);
            return true;
        }
    }
}