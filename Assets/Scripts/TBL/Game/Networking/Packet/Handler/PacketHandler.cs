using System;
using System.Collections.Generic;
namespace TBL.Game.Networking
{
    public partial class PacketHandler
    {
        public delegate bool Deserializer(string data, out IPacket packet);
        record DeserializeSetting(Deserializer Deserializer, Action<IPacket> Event);
        Dictionary<PacketType, DeserializeSetting> DeserializerDict { get; } = new();

        public PacketHandler()
        {
            // Regist!
            DeserializerDict.Add(
                PacketType.FinishedQuest,
                new(FinishedQuestPacket.Deserializer,
                    packet => FinishedQuestPacketEvent.Invoke(packet as FinishedQuestPacket))
            );
            DeserializerDict.Add(
                PacketType.PassCard,
                new(PassCardPacket.Deserializer,
                    packet => PassCardPacketEvent.Invoke(packet as PassCardPacket))
            );
            DeserializerDict.Add(
                PacketType.AcceptCard,
                new(AcceptCardPacket.Deserializer,
                    packet => AcceptCardPacketEvent(packet as AcceptCardPacket))
            );
            DeserializerDict.Add(
                PacketType.RejectCard,
                new(RejectCardPacket.Deserializer,
                    packet => RejectCardPacketEvent(packet as RejectCardPacket))
            );
            DeserializerDict.Add(
                PacketType.NewRound,
                new(NewRoundPacket.Deserializer,
                    packet => NewRoundPacketEvent(packet as NewRoundPacket))
            );
        }

        public void OnClientPacket(PacketType type, string data)
        {
            if (CustomDeserializer(type, data))
                return;

            switch (type)
            {
                case PacketType.GameStart:
                    {
                        data.Deserialize(out GameStartPacket packet);
                        GameStartPacketEvent(packet);
                        break;
                    }

                case PacketType.PlayerStatus:
                    {
                        data.Deserialize(out PlayerStatusPacket packet);
                        PlayerStatusPacketEvent(packet);
                        break;
                    }

                case PacketType.ChangePhase:
                    {
                        data.Deserialize(out ChangePhasePacket packet);
                        ChangePhasePacketEvent(packet);
                        break;
                    }
            }
        }

        public void OnServerPacket(PacketType type, string data)
        {
            if (CustomDeserializer(type, data))
                return;

            switch (type)
            {
                case PacketType.PlayerReady:
                    {
                        data.Deserialize(out PlayerReadyPacket packet);
                        PlayerReadyPacketEvent(packet);
                        break;
                    }
            }
        }

        bool CustomDeserializer(PacketType type, string data)
        {
            if (DeserializerDict.ContainsKey(type))
            {
                var setting = DeserializerDict[type];
                if (setting.Deserializer(data, out var packet))
                    setting.Event(packet);
                else
                    throw new Exception($"Deserialize {type} Failed!");

                return true;
            }

            return false;
        }
    }
}
