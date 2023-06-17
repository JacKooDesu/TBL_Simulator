using UnityEngine;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Mirror;
using Cysharp.Threading.Tasks;

namespace TBL.Game.Sys
{
    using Game.Networking;
    public interface IPlayerStandalone
    {
        Player player { get; }
        bool IsReady { get; }
        void SetReady();
        /// <summary>
        /// 玩家真正的順位
        /// </summary>
        int Index { get; }
        PacketHandler PacketHandler { get; }
        void Initialize();
        void Send<T>(SendType sendType, T packet) where T : IPacket;


        public static IPlayerStandalone Me { get; set; }
        public static Player MyPlayer => Me?.player;
        public static ReadOnlyCollection<IPlayerStandalone> Standalones => standalones.AsReadOnly();
        private static List<IPlayerStandalone> standalones = new();
        public static void Regist(IPlayerStandalone p) => standalones.Add(p);

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        private static void ClearStatic() => standalones = new();
#endif
    }
}