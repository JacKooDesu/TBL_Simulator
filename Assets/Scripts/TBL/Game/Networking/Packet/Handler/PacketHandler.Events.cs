using System;
namespace TBL.Game.Networking
{
    public partial class PacketHandler
    {
        #region Client
        public event Action<PlayerStatusPacket> PlayerStatusPacketEvent = _ => { };
        public event Action<ChangePhasePacket> ChangePhasePacketEvent = _ => { };
        #endregion

        #region Server
        public event Action<FinishedQuestPacket> FinishedQuestPacketEvent = _ => { };
        #endregion
    }
}
