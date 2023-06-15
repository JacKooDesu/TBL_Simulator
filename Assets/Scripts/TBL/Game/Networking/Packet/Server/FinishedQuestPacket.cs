using System;
namespace TBL.Game.Networking
{
    using QuestType = PhaseQuestStatus.QuestType;
    public class FinishedQuestPacket : IPacket
    {
        public QuestType quest { get; private set; }

        public FinishedQuestPacket(QuestType quest) => this.quest = quest;
        public bool Serialize(ref string data)
        {
            data = ((int)quest).ToString();
            return true;
        }

        public PacketType Type() => PacketType.FinishedQuest;

        public static PacketHandler.Deserializer Deserializer => Deserialize;
        static bool Deserialize(string data, out IPacket packet)
        {
            packet = null;
            if (Int32.TryParse(data, out var questInt))
            {
                packet = new FinishedQuestPacket((QuestType)questInt);
                return true;
            }

            return false;
        }
    }
}