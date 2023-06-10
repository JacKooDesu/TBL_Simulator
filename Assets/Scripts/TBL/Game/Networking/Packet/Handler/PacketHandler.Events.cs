using System;
namespace TBL.Game.Networking
{
    public partial class PacketHandler
    {
        public event Action<PlayerStatusPacket> PlayerStatusPacketEvent = _ => { };
        public event Action<ChangePhasePacket> ChangePhasePacketEvent = _ => { };
    }
}
