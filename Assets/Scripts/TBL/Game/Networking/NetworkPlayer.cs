using UnityEngine;
using System;
using Mirror;
using Cysharp.Threading.Tasks;

namespace TBL.Game.Networking
{
    using Sys;
    using Game.Networking;
    using NetworkManager = Game.Networking.NetworkRoomManager;
    using Game.UI.Main;
    /// <summary>
    /// 基本只拿來傳訊
    /// </summary>
    public class NetworkPlayer : NetworkBehaviour, IPlayerStandalone
    {
        public Player player { get; private set; }
        [SyncVar] bool isReady = false;
        public bool IsReady => isReady;
        [Server] public void SetReady() => isReady = true;

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
            IPlayerStandalone.Regist(this);

            PacketHandler.ServerReadyPacketEvent += _ =>
                Send(SendType.Cmd, new PlayerReadyPacket());

            if (isLocalPlayer)
                packetHandler.GameStartPacketEvent += _ => MainUIManager.Singleton?.SetupUI(this);
            packetHandler.PlayerStatusPacketEvent += p => player.UpdateStatus(p.Data);
        }

        [ClientRpc]
        public void RpcSend(PacketType type, string data)
        {
            packetHandler.OnClientPacket(type, data);
        }

        [TargetRpc]
        public void TargetSend(PacketType type, string data)
        {
            packetHandler.OnClientPacket(type, data);
        }

        [Command]
        public void CmdSend(PacketType type, string data)
        {
            packetHandler.OnServerPacket(type, data);
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

        public void Send<P>(SendType sendType, P packet) where P : IPacket
        {
            if (!isServer && sendType != SendType.Cmd)
            {
                sendType = SendType.Cmd;
                Debug.LogWarning("[Standalone] Trying send non-cmd function on client!");
            }

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
