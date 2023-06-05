using UnityEngine;
using System;
using Mirror;
using Cysharp.Threading.Tasks;

namespace TBL.Game.Networking
{
    using Sys;
    /// <summary>
    /// 基本只拿來傳訊
    /// </summary>
    public class NetworkPlayer : NetworkBehaviour, IPlayerStandalone
    {
        public Player player { get; private set; }
        [SyncVar] int index;
        public int Index => index;
        public event Action<string> OnCmd;
        public event Action<string> OnRpc;
        public event Action<string> OnTarget;
        PacketHandler packetHandler = new();
        public PacketHandler PacketHandler => packetHandler;

        public override void OnStartClient() => Initialize();
        public void Initialize()
        {
            player = new(this);
            if (isLocalPlayer) IPlayerStandalone.Me = this;
        }

        [ClientRpc, Server]
        public void RpcSend(PacketType type, string data)
        {
            packetHandler.OnPacket(type, data);
        }

        [TargetRpc, Server]
        public void TargetSend(PacketType type, string data)
        {
            packetHandler.OnPacket(type, data);
        }

        [Command]
        public void CmdSend(PacketType type, string data)
        {
            packetHandler.OnPacket(type, data);
        }

        [Server]
        public async UniTask Request<T>(PacketType type, string data)
        {
            TargetSend(type, data);
            bool isFinished = false;
            Action<string> awaiter = (_) => isFinished = true;
            OnCmd += awaiter;
            await UniTask.WaitUntil(() => isFinished);
            OnCmd -= awaiter;
        }

        public void Send(SendType sendType, IPacket packet)
        {
            if (isClient && sendType != SendType.Cmd)
                sendType = SendType.Cmd;

            var data = "";
            packet.Serialize(ref data);
            Action<PacketType, string> action = sendType switch
            {
                SendType.Cmd => CmdSend,
                SendType.Rpc => RpcSend,
                SendType.Target => TargetSend,
                _ => (_, _) => Debug.Log($"Unknown Send Type!")
            };
            action(packet.Type(), data);
        }
    }

    public enum SendType
    {
        Rpc,
        Cmd,
        Target
    }
}
