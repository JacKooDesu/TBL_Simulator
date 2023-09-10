using System;
using UnityEngine.Events;

namespace TBL.Game.Networking
{
    public partial class PacketHandler
    {
        #region Client
        public UnityEvent<ServerReadyPacket> ServerReadyPacketEvent { get; } = new();
        public UnityEvent<GameStartPacket> GameStartPacketEvent { get; } = new();
        public UnityEvent<PlayerStatusPacket> PlayerStatusPacketEvent { get; } = new();
        public UnityEvent<ChangePhasePacket> ChangePhasePacketEvent { get; } = new();
        public UnityEvent<NewRoundPacket> NewRoundPacketEvent { get; } = new();
        public UnityEvent<ActionRequestPacket> ActionRequestPacketEvent { get; } = new();
        #endregion

        #region Server
        public UnityEvent<PlayerReadyPacket> PlayerReadyPacketEvent { get; } = new();
        public UnityEvent<PassCardPacket> PassCardPacketEvent { get; } = new();
        public UnityEvent<FinishedQuestPacket> FinishedQuestPacketEvent { get; } = new();
        public UnityEvent<AcceptCardPacket> AcceptCardPacketEvent { get; } = new();
        public UnityEvent<RejectCardPacket> RejectCardPacketEvent { get; } = new();
        public UnityEvent<UseCardPacket> UseCardPacketEvent { get; } = new();
        public UnityEvent<UseSkillPacket> UseSkillPacketEvent { get; } = new();
        public UnityEvent<ActionResponsePacket> ActionResponsePacketEvent { get; } = new();
        #endregion
    }
}
