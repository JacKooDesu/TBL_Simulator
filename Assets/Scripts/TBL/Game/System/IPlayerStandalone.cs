using UnityEngine;
using System;
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
        void Send(SendType sendType, IPacket packet);
        public static IPlayerStandalone Me { get; set; }
    }
}