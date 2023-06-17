using System;
namespace TBL.Game.Networking
{
    public class AcceptCardPacket : IPacket
    {
        public PacketType Type() => PacketType.AcceptCard;

        public bool Serialize(ref string data)
        {
            return true;
        }

        public static PacketHandler.Deserializer Deserializer => Deserialize;
        static bool Deserialize(string data, out IPacket packet)
        {
            packet = new AcceptCardPacket();
            return true;
        }
    }
}
