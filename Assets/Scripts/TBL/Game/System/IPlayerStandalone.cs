using UnityEngine;
using System;
using Mirror;
using Cysharp.Threading.Tasks;

namespace TBL.Game.Sys
{
    using Game.Networking;
    public interface IPlayerStandalone
    {
        void Send<T>(SendType sendType, IPacket<T> packet);
    }
}