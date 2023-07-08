using System;
namespace TBL.Game.Networking
{
    using Sys;
    public class ActionRequestPacket : IPacket
    {
        const char SPLIT_CHAR = '|';

        public ActionRequestPacket(ActionType actionType, string data)
        {
            ActionType = actionType;
            Data = data;
        }

        public PacketType Type() => PacketType.ActionRequest;
        public ActionType ActionType { get; private set; }
        public string Data { get; private set; }

        public bool Serialize(ref string data)
        {
            data = ((int)ActionType).ToString() + SPLIT_CHAR + Data;
            return true;
        }

        public static PacketHandler.Deserializer Deserializer => Deserialize;
        static bool Deserialize(string data, out IPacket packet)
        {
            packet = null;
            var datas = data.Split(SPLIT_CHAR);
            if (!Int32.TryParse(datas[0], out var type))
                return false;
            packet = new ActionRequestPacket((ActionType)type, datas[1]);
            return true;
        }
    }
}