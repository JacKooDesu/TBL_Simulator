using System;
namespace TBL.Game.Networking
{
    public class UseCardPacket : IPacket
    {
        public CardEnum.Property card { get; set; }
        public PacketType Type() => PacketType.UseCard;
        public UseCardPacket(CardEnum.Property card)
        {
            this.card = card;
        }

        public bool Serialize(ref string data)
        {
            data = $"{(int)card}";
            return true;
        }

        public static PacketHandler.Deserializer Deserializer => Deserialize;
        static bool Deserialize(string data, out IPacket packet)
        {
            packet = null;
            if (Int32.TryParse(data, out var card))
            {
                packet = new UseCardPacket((CardEnum.Property)card);
                return true;
            }

            return false;
        }
    }
}