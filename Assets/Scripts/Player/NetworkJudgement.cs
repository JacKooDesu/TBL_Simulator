using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

namespace TBL
{
    public class NetworkJudgement : NetworkBehaviour
    {
        public SyncList<int> netIdList = new SyncList<int>();
        // net hand card callback
        public void OnUpdateNetIdList(SyncList<int>.Operation op, int index, int oldItem, int newItem)
        {
            switch (op)
            {
                case SyncList<int>.Operation.OP_ADD:
                    // index is where it got added in the list
                    // newItem is the new item

                    break;
                case SyncList<int>.Operation.OP_CLEAR:
                    // list got cleared
                    break;
                case SyncList<int>.Operation.OP_INSERT:
                    // index is where it got added in the list
                    // newItem is the new item
                    break;
                case SyncList<int>.Operation.OP_REMOVEAT:
                    // index is where it got removed in the list
                    // oldItem is the item that was removed
                    break;
                case SyncList<int>.Operation.OP_SET:
                    // index is the index of the item that was updated
                    // oldItem is the previous value for the item at the index
                    // newItem is the new value for the item at the index
                    break;
            }

            List<NetworkPlayer> newPlayerList = new List<NetworkPlayer>();
            foreach (int i in netIdList)
            {
                foreach (NetworkPlayer p in manager.players)
                {
                    if ((int)p.netId == i)
                        newPlayerList.Add(p);
                }
            }

            manager.players = newPlayerList;
            netCanvas.InitPlayerStatus();
        }

        [SerializeField, SyncVar(hook = nameof(OnTimeChange))] int timer;
        void OnTimeChange(int oldTime, int newTime)
        {

        }

        [SerializeField, SyncVar(hook = nameof(OnCurrentPlayerChange))] int currentPlayerIndex;
        void OnCurrentPlayerChange(int oldPlayer, int newPlayer)
        {

        }


        TBL.NetCanvas.GameScene netCanvas;
        NetworkRoomManager manager;

        private void Start()
        {
            netCanvas = FindObjectOfType<TBL.NetCanvas.GameScene>();
            manager = ((NetworkRoomManager)NetworkManager.singleton);

            netIdList.Callback += OnUpdateNetIdList;

            StartCoroutine(WaitAllPlayerConnect());
        }

        IEnumerator WaitAllPlayerConnect()
        {
            while (manager.players.Count != manager.roomSlots.Count)
                yield return null;

            // 排序
            NetworkPlayer tempPlayer;

            for (int i = 0; i < manager.players.Count; ++i)
            {
                for (int j = 0; j < manager.players.Count - 1; ++j)
                {
                    if (manager.players[j].playerIndex > manager.players[j + 1].playerIndex)
                    {
                        tempPlayer = manager.players[j];
                        manager.players[j] = manager.players[j + 1];
                        manager.players[j + 1] = tempPlayer;
                    }
                }
            }

            netCanvas.InitPlayerMapping();
            print("Player init finished");
            foreach (NetworkPlayer p in manager.players)
            {
                p.InitPlayer();
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

        }
    }

}
