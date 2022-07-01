using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace TBL
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
            // if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GameScene")
            //     return;

            // if (!hasGenerateUI)
            // {
            //     playerUI = Instantiate(playerUIPrefab);
            //     GameObject g = FindObjectOfType<RoomScene>().playerListUI;
            //     playerUI.transform.SetParent(g.transform);
            //     playerUI.transform.localScale = Vector3.one;
            //     hasGenerateUI = true;
            // }

            // // 效能問題 ?
            // if (isLocalPlayer)
            // {
            //     if (playerName != GameUtils.PlayerName)
            //     {
            //         CmdSetName();
            //     }
            // }
            // playerUI.GetComponentInChildren<UnityEngine.UI.Text>().text = playerName;

            // UnityEngine.UI.Image statusImage = playerUI.transform.Find("Ready").GetComponent<UnityEngine.UI.Image>();
            // if (readyToBegin)
            //     statusImage.color = new Color(0.1056606f, 1f, 0);
            // else
            //     statusImage.color = new Color(1f, 0, 0);
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
            // base.OnGUI();
        }
    }
}

