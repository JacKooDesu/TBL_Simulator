using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TBL.NetCanvas
{
    public class RoomScene : NetCanvasBinderBase
    {
        [SerializeField]
        Button ready = default;
        [SerializeField]
        Button cancel = default;
        [SerializeField]
        Button start = default;
        public Transform playerListUiParent = default;
        public Transform playerUiPrefab;    // 需使用場景中物件

        protected override void Start()
        {
            base.Start();

            ready.onClick.AddListener(() => manager.LocalRoomPlayer.CmdChangeReadyState(true));

            cancel.onClick.AddListener(() => manager.LocalRoomPlayer.CmdChangeReadyState(false));
        }

        public void UpdatePlayerList()
        {
            print($"System - 更新玩家列表 | 目前玩家數 {manager.roomSlots.Count}");
            start.interactable = false;

            // Clear Prefabs
            for (int i = playerListUiParent.childCount - 1; i >= 0; --i)
                Destroy(playerListUiParent.GetChild(i).gameObject);

            foreach (var p in manager.roomSlots)
            {
                var player = p as NetworkRoomPlayer;
                var pObject = Instantiate(playerUiPrefab, playerListUiParent);
                pObject.GetComponentInChildren<Text>().text = player.playerName;
                pObject.transform.Find("Ready").GetComponent<Image>().color = p.readyToBegin ? Color.green : Color.red;

                if (p.isServer)
                {
                    start.onClick.RemoveAllListeners();
                    start.onClick.AddListener(() => manager.ServerChangeScene(manager.GameplayScene));
                    start.interactable = true;
                }
            }
        }
    }

}
