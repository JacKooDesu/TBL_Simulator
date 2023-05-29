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
    public class LocalPlayer : MonoBehaviour, IPlayerStandalone
    {
        int index;
        public int Index => index;

        event Action onCmd;
        event Action onRpc;
        event Action onTarget;

        bool isClient;

        public void Setup(int index, bool isClient)
        {
            
        }

        public void RpcSend(string data)
        {
            onRpc.Invoke();
        }

        public void TargetSend(string data)
        {
            onTarget.Invoke();
        }

        public void CmdSend(string data)
        {
            onCmd.Invoke();
        }

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
