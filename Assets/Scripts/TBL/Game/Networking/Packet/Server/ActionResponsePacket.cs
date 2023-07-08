using System;

namespace TBL.Game.Networking
{
    using TBL.Game.Sys;
    public class ActionResponsePacket : IPacket
    {

        public string Data { get; set; }
        public ActionType ActionType { get; private set; }
        public PacketType Type() => PacketType.ActionResponse;
        const char SPLIT_CHAR = '|';
        public bool Serialize(ref string data)
        {
            data = ((int)ActionType).ToString() + SPLIT_CHAR + Data;
            return true;
        }

        public ActionResponsePacket WithData(string data)
        {
            this.Data = data;
            return this;
        }
        public ActionResponsePacket WithType(Sys.ActionType type)
        {
            this.ActionType = type;
            return this;
        }

        public static PacketHandler.Deserializer Deserializer => Deserialize;
        static bool Deserialize(string data, out IPacket packet)
        {
            packet = null;
            var datas = data.Split(SPLIT_CHAR);
            if (!Int32.TryParse(datas[0], out var type))
                return false;
            packet = new ActionResponsePacket()
                            .WithType((ActionType)type)
                            .WithData(datas[1]);
            return true;
        }
    }
}