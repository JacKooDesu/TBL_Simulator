using System;
namespace TBL.Game.Networking
{
    public partial class PacketHandler
    {
        #region Client
        public event Action<GameStartPacket> GameStartPacketEvent = _ => { };
        public event Action<PlayerStatusPacket> PlayerStatusPacketEvent = _ => { };
        public event Action<ChangePhasePacket> ChangePhasePacketEvent = _ => { };
        #endregion

        #region Server
        public event Action<PlayerReadyPacket> PlayerReadyPacketEvent = _ => { };
        public event Action<PassCardPacket> PassCardPacketEvent = _ => { };
        public event Action<FinishedQuestPacket> FinishedQuestPacketEvent = _ => { };
        public event Action<AcceptCardPacket> AcceptCardPacketEvent = _ => { };
        public event Action<RejectCardPacket> RejectCardPacketEvent = _ => { };
        #endregion
    }
}
