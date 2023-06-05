using System;
namespace TBL.Game.Networking
{
    public partial class PacketHandler
    {
        public event Action<PlayerStatusPacket> PlayerStatusPacketEvent = _ => { };
    }
}
