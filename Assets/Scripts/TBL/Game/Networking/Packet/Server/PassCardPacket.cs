using System;
namespace TBL.Game.Networking
{
    using TBL.Game.Sys;
    using QuestType = PhaseQuestStatus.QuestType;
    public class PassCardPacket : IPacket
    {
        public int cardId;
        public int target;

        public PassCardPacket(int cardId, int target)
        {
            this.cardId = cardId;
            this.target = target;
        }
        public bool Serialize(ref string data)
        {
            data = $"{cardId},{target}";
            return true;
        }

        public PacketType Type() => PacketType.PassCard;

        public static PacketHandler.Deserializer Deserializer => Deserialize;
        static bool Deserialize(string data, out IPacket packet)
        {
            packet = null;
            var array = data.Split(',');
            if (Int32.TryParse(array[0], out var cardId) &&
                Int32.TryParse(array[1], out var target))
            {
                packet = new PassCardPacket(cardId, target);
                return true;
            }

            return false;
        }
    }
}