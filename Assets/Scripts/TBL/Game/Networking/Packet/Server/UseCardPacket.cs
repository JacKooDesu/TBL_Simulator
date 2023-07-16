using System;
namespace TBL.Game.Networking
{
    public class UseCardPacket : IPacket
    {
        public int cardId { get; set; }
        public PacketType Type() => PacketType.UseCard;
        public UseCardPacket(int cardId)
        {
            this.cardId = cardId;
        }

        public bool Serialize(ref string data)
        {
            data = $"{cardId}";
            return true;
        }

        public static PacketHandler.Deserializer Deserializer => Deserialize;
        static bool Deserialize(string data, out IPacket packet)
        {
            packet = null;
            if (Int32.TryParse(data, out var cardId))
            {
                packet = new UseCardPacket(cardId);
                return true;
            }

            return false;
        }
    }
}