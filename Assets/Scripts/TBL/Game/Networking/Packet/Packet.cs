using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;

namespace TBL.Game.Networking
{
    // TODO: currently using JsonUtility for testing, should use byte convert?
    public static class PacketUtil
    {
        public static string Serialize(IPacket packet)
        {
            return JsonConvert.SerializeObject(packet);
        }

        public static IPacket Deserialize(this string data)
        {
            IPacket result = JsonConvert.DeserializeObject<IPacket>(data);
            return result;
        }

        public static bool Deserialize<T>(this string data, out T result)
        where T : IPacket
        {
            try
            {
                result = JsonConvert.DeserializeObject<T>(data);
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

    public enum PacketType
    {
        // Client
        _CLIENT_ = 1,
        Bundled,
        GameStart,
        PlayerStatus,
        ChangePhase,

        // Server
        _SERVER_ = 1000,
        PlayerReady,
        FinishedQuest,
        PassCard,
    }

    public interface IPacket
    {
        PacketType Type();
        bool Serialize(ref string data);
    }

    // [JsonObject]
    // public class BundledPacket : IPacket<List<object>>
    // {
    //     public List<object> Data { get; private set; }
    //     public PacketType Type => TYPE;
    //     const PacketType TYPE = PacketType.Bundled;

    //     public BundledPacket(params IPacket[] packets) => this.Data = new(packets);
    //     public void Add(params IPacket[] packets) => Data.AddRange(packets);

    //     public bool Serialize(ref string data)
    //     {
    //         data = PacketUtil.Serialize(this);
    //         return true;
    //     }
    // }
}
