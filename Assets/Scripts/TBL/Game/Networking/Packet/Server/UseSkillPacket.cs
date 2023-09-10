using System;
using TBL.Game.Hero;

namespace TBL.Game.Networking
{
    public class UseSkillPacket : IPacket
    {
        public PacketType Type() => PacketType.UseSkill;
        public HeroId HeroId { get; set; }
        public int Id { get; set; }

        public UseSkillPacket(HeroId heroId, int id)
        {
            HeroId = heroId;
            Id = id;
        }

        public bool Serialize(ref string data)
        {
            data = $"{(int)HeroId},{Id}";
            return true;
        }

        public static PacketHandler.Deserializer Deserializer => Deserialize;
        static bool Deserialize(string data, out IPacket packet)
        {
            packet = null;
            var values = data.Split(',');
            if (!Int32.TryParse(values[1], out var sId) ||
                !Int32.TryParse(values[0], out var hId))
                return false;

            packet = new UseSkillPacket((HeroId)hId, sId);
            return true;
        }
    }
}