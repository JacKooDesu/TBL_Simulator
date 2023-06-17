using System;

namespace TBL.Game.Networking
{
    public class RejectCardPacket : IPacket
    {
        public PacketType Type() => PacketType.RejectCard;
        public bool Serialize(ref string data)
        {
            return true;
        }

        public static PacketHandler.Deserializer Deserializer => Deserialize;
        static bool Deserialize(string data, out IPacket packet)
        {
            packet = new RejectCardPacket();
            return true;
        }
    }
}