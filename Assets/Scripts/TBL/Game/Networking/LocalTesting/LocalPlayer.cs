using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

namespace TBL.Game.Networking
{
    using Sys;
    using TBL.Utils;
    using UI.Main;
    /// <summary>
    /// 基本只拿來傳訊
    /// </summary>
    public class LocalPlayer : MonoBehaviour, IPlayerStandalone
    {
        public Player player { get; private set; }
        public bool IsReady { get; private set; } = true;
        public void SetReady() => IsReady = true;
        int index;
        public int Index => index;

        event Action onCmd;
        event Action onRpc;
        event Action onTarget;

        PacketHandler packetHandler = new();
        public PacketHandler PacketHandler => packetHandler;

        bool isClient;
        [SerializeField] bool isLocal;   // for debug
        // void Start() => Initialize();
        public void Initialize()
        {
            player = new(this);
            if (isLocal) IPlayerStandalone.Me = this;
            IPlayerStandalone.Regist(this);
            Send(SendType.Cmd, new PlayerReadyPacket());

            // MainUIManager.Singleton?.SetupUI(this);
            if (isLocal)
                packetHandler.GameStartPacketEvent.AutoRemoveListener(_ => MainUIManager.Singleton?.SetupUI(this));
            packetHandler.PlayerStatusPacketEvent.AddListener(p => player.UpdateStatus(p.Data));
        }

        public void RpcSend(PacketType type, string data)
        {
            packetHandler.OnClientPacket(type, data);
        }

        public void TargetSend(PacketType type, string data)
        {
            packetHandler.OnClientPacket(type, data);
        }

        public void CmdSend(PacketType type, string data)
        {
            packetHandler.OnServerPacket(type, data);
        }

        public async UniTask Request<T>(PacketType type, string data)
        {
            TargetSend(type, data);
            bool isFinished = false;
            Action awaiter = () => isFinished = true;
            onCmd += awaiter;
            await UniTask.WaitUntil(() => isFinished);
            onCmd -= awaiter;
        }

        public void Send<P>(SendType sendType, P packet) where P : IPacket
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
}
