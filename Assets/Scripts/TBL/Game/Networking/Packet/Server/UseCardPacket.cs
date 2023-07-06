using System;
namespace TBL.Game.Networking
{
    public class UseCardPacket : IPacket
    {

        public CardEnum.Property card { get; set; }
        public int target { get; set; } = -1;
        public PacketType Type() => PacketType.UseCard;
        public UseCardPacket(CardEnum.Property card, int? target = null)
        {
            this.card = card;
            this.target = target ?? -1;
        }

        public bool Serialize(ref string data)
        {
            data = $"{(int)card},{target}";
            return true;
        }

        public static PacketHandler.Deserializer Deserializer => Deserialize;
        static bool Deserialize(string data, out IPacket packet)
        {
            packet = null;
            var array = data.Split(',');
            if (Int32.TryParse(array[0], out var card) &&
                Int32.TryParse(array[1], out var target))
            {
                packet = new UseCardPacket((CardEnum.Property)card, target);
                return true;
            }

            return false;
        }
    }
}