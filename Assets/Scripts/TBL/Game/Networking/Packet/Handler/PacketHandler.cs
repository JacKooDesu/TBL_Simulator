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
                    packet => AcceptCardPacketEvent.Invoke(packet as AcceptCardPacket))
            );
            DeserializerDict.Add(
                PacketType.RejectCard,
                new(RejectCardPacket.Deserializer,
                    packet => RejectCardPacketEvent.Invoke(packet as RejectCardPacket))
            );
            DeserializerDict.Add(
                PacketType.NewRound,
                new(NewRoundPacket.Deserializer,
                    packet => NewRoundPacketEvent.Invoke(packet as NewRoundPacket))
            );
            DeserializerDict.Add(
                PacketType.UseCard,
                new(UseCardPacket.Deserializer,
                    packet => UseCardPacketEvent.Invoke(packet as UseCardPacket))
            );
        }

        public void OnClientPacket(PacketType type, string data)
        {
            if (CustomDeserializer(type, data))
                return;

            switch (type)
            {
                case PacketType.ServerReady:
                    {
                        data.Deserialize(out ServerReadyPacket packet);
                        ServerReadyPacketEvent.Invoke(packet);
                        break;
                    }
                case PacketType.GameStart:
                    {
                        data.Deserialize(out GameStartPacket packet);
                        GameStartPacketEvent.Invoke(packet);
                        break;
                    }

                case PacketType.PlayerStatus:
                    {
                        data.Deserialize(out PlayerStatusPacket packet);
                        PlayerStatusPacketEvent.Invoke(packet);
                        break;
                    }

                case PacketType.ChangePhase:
                    {
                        data.Deserialize(out ChangePhasePacket packet);
                        ChangePhasePacketEvent.Invoke(packet);
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
                        PlayerReadyPacketEvent.Invoke(packet);
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
