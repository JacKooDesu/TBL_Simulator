using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;

// FIXME: The packet system still very buggy,
// FIXME: need be planned how to send/serialize/deserialize between data & packet!
namespace TBL.Game.Networking
{
    // TODO: currently using JsonUtility for testing, should use byte convert?
    public static class PacketUtil
    {
        public static string Serialize<T>(IPacket<T> packet)
        {
            return JsonConvert.SerializeObject(packet);
        }

        public static string Serialize(IPacket packet)
        {
            return JsonConvert.SerializeObject(packet);
        }


        public static T Deserialize<T>(string data)
        where T : IPacket
        {
            T result = JsonConvert.DeserializeObject<T>(data);
            return result;
        }

        public static IPacket Deserialize(string data)
        {
            IPacket result = JsonConvert.DeserializeObject<IPacket>(data);
            return result;
        }

        public static bool Deserialize<T>(string data, out T result)
        where T : IPacket
        {
            try
            {
                result = JsonUtility.FromJson<T>(data);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                result = default;
                return false;
            }
            return true;
        }
    }

    public interface IPacket<T> : IPacket
    {
        T Data { get; }
        PacketType Type { get; }
        bool Serialize(ref string data);
    }

    public enum PacketType
    {
        Bundled,
        PlayerStatus,
    }

    public interface IPacket { }

    [JsonObject]
    public class BundledPacket : IPacket<List<object>>
    {
        public List<object> Data { get; private set; }
        public PacketType Type => TYPE;
        const PacketType TYPE = PacketType.Bundled;

        public BundledPacket(params IPacket[] packets) => this.Data = new(packets);
        public void Add(params IPacket[] packets) => Data.AddRange(packets);

        public bool Serialize(ref string data)
        {
            data = PacketUtil.Serialize(this);
            return true;
        }
    }
}
