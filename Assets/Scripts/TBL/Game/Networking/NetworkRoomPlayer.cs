using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace TBL.Game.Networking
{
    public class NetworkRoomPlayer : Mirror.NetworkRoomPlayer
    {
        [SyncVar(hook = nameof(OnPlayerNameChange))] public string playerName;
        void OnPlayerNameChange(string oldName, string newName)
        {
            if (roomUi == null)
                roomUi = FindObjectOfType<NetCanvas.RoomScene>();

            roomUi?.UpdatePlayerList();
        }

        NetCanvas.RoomScene roomUi;

        [SerializeField]
        GameObject playerUIPrefab = default;
        GameObject playerUI;
        bool hasGenerateUI = false;

        private void OnEnable()
        {

        }

        private void Update()
        {

        }

        public override void OnStartServer()
        {
            playerName = (string)connectionToClient.authenticationData;
        }

        public override void OnClientEnterRoom()
        {
            if (roomUi == null)
                roomUi = FindObjectOfType<NetCanvas.RoomScene>();

            roomUi?.UpdatePlayerList();

            base.OnClientEnterRoom();
        }

        public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
        {
            roomUi?.UpdatePlayerList();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Destroy(playerUI);
        }

        [Command]
        public void CmdSetName()
        {
            playerName = GameUtils.PlayerName;
        }

        public override void OnGUI()
        {
            base.OnGUI();
        }
    }
}

