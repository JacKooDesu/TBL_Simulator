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
        int Index { get; }
        void Initialize();
        void Send(SendType sendType, IPacket packet);
        public static IPlayerStandalone Me { get; set; }
    }
}