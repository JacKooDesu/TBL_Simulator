using System;
namespace TBL.Game.Networking
{
    public class NewRoundPacket : IPacket
    {
        public PacketType Type() => PacketType.NewRound;
        public int HostId { get; private set; }

        public bool Serialize(ref string data)
        {
            data = HostId.ToString();
            return true;
        }

        public static PacketHandler.Deserializer Deserializer => Deserialize;
        static bool Deserialize(string data, out IPacket packet)
        {
            packet = null;

            if (!Int32.TryParse(data, out var id))
                return false;
            
            packet = new NewRoundPacket { HostId = id };
            return true;
        }
    }
}