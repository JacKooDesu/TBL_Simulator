using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

namespace TBL.Testing
{
    using Game.Networking;
    public class PacketTest
    {
        [Test]
        public void PlayerStatusPacket()
        {
            var packet = new PlayerStatusPacket(default);
            var result = "";
            packet.Serialize(ref result);
            Debug.Log(result);
            packet = PacketUtil.Deserialize<PlayerStatusPacket>(result);
        }

        [Test]
        public void BundledPacket()
        {
            var packet = new BundledPacket(
                new PlayerStatusPacket(default)
            );
            packet.Add(new PlayerStatusPacket(default));

            string result = "";
            packet.Serialize(ref result);
            Debug.Log(result);
            packet = PacketUtil.Deserialize<BundledPacket>(result);
        }
    }
}

