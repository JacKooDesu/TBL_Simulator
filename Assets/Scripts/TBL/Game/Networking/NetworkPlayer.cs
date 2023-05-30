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

        public override void OnStartClient() => Initialize();
        public void Initialize()
        {
            player = new(this);
            if (isLocalPlayer) IPlayerStandalone.Me = this;
        }

        [ClientRpc, Server]
        public void RpcSend(string data)
        {
            OnRpc.Invoke(data);
        }

        [TargetRpc, Server]
        public void TargetSend(string data)
        {
            OnTarget.Invoke(data);
        }

        [Command]
        public void CmdSend(string data)
        {
            OnCmd.Invoke(data);
        }

        [Server]
        public async UniTask Request<T>(string data)
        {
            TargetSend(data);
            bool isFinished = false;
            Action<string> awaiter = (_) => isFinished = true;
            OnCmd += awaiter;
            await UniTask.WaitUntil(() => isFinished);
            OnCmd -= awaiter;
        }

        public void Send<T>(SendType sendType, IPacket<T> packet)
        {
            if (isClient && sendType != SendType.Cmd)
                sendType = SendType.Cmd;

            var data = "";
            packet.Serialize(ref data);
            Action<string> action = sendType switch
            {
                SendType.Cmd => CmdSend,
                SendType.Rpc => RpcSend,
                SendType.Target => TargetSend,
                _ => _ => Debug.Log($"Unknown Send Type!")
            };
            action(data);
        }
    }

    public enum SendType
    {
        Rpc,
        Cmd,
        Target
    }
}
