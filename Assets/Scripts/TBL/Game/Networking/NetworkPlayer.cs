using UnityEngine;
using System;
using Mirror;
using Cysharp.Threading.Tasks;

namespace TBL.Game.Networking
{
    /// <summary>
    /// 基本只拿來傳訊
    /// </summary>
    public class NetworkPlayer : NetworkBehaviour
    {
        [SyncVar] int index;
        public int Index => index;

        event Action onCmd;
        event Action onRpc;
        event Action onTarget;

        public enum SendType
        {
            Rpc,
            Cmd,
            Target
        }

        [ClientRpc, Server]
        public void RpcSend(string data)
        {
            onRpc.Invoke();
        }

        [TargetRpc, Server]
        public void TargetSend(string data)
        {
            onTarget.Invoke();
        }

        [Command]
        public void CmdSend(string data)
        {
            onCmd.Invoke();
        }

        [Server]
        public async UniTask Request<T>(string data)
        {
            TargetSend(data);
            bool isFinished = false;
            Action awaiter = () => isFinished = true;
            onCmd += awaiter;
            await UniTask.WaitUntil(() => isFinished);
            onCmd -= awaiter;
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
}
