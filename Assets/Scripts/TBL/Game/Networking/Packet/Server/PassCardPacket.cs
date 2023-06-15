using System;
namespace TBL.Game.Networking
{
    using TBL.Game.Sys;
    using QuestType = PhaseQuestStatus.QuestType;
    public class PassCardPacket : IPacket
    {
        public CardEnum.Property card;
        public int target;

        public PassCardPacket(CardEnum.Property card, int target)
        {
            this.card = card;
            this.target = target;
        }
        public bool Serialize(ref string data)
        {
            data = $"{(int)card},{target}";
            return true;
        }

        public PacketType Type() => PacketType.PassCard;

        public static PacketHandler.Deserializer Deserializer => Deserialize;
        static bool Deserialize(string data, out IPacket packet)
        {
            packet = null;
            var array = data.Split(',');
            if (Int32.TryParse(array[0], out var card) &&
                Int32.TryParse(array[1], out var target))
            {
                packet = new PassCardPacket((CardEnum.Property)card, target);
                return true;
            }

            return false;
        }
    }
}