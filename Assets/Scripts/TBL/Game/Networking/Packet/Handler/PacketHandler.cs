using System;
namespace TBL.Game.Networking
{
    public partial class PacketHandler
    {
        public void OnPacket(PacketType type, string data)
        {
            switch (type)
            {
                case PacketType.PlayerStatus:
                    data.Deserialize(out PlayerStatusPacket packet);
                    PlayerStatusPacketEvent(packet);
                    break;
            }
        }
    }
}
