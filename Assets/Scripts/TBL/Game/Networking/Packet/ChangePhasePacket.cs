#nullable enable
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TBL.Game.Networking
{
    using Game.Sys;
    [JsonObject]
    public class ChangePhasePacket : IPacket
    {
        public PacketType Type() => PacketType.ChangePhase;

        public ChangePhasePacket(PhaseType type)
        {
            this.PhaseType = type;
        }

        public bool Serialize(ref string data)
        {
            data = PacketUtil.Serialize(this);
            return true;
        }

        #region JSON_PROPERTY
        [JsonProperty]
        public PhaseType PhaseType { get; private set; }
        [JsonProperty]
        public float SendAt { get; private set; }   // FIXME: 是否需要傳精確時間??
        #endregion
    }
}