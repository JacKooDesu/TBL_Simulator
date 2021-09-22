using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

namespace TBL
{
    public class NetworkJudgement : NetworkBehaviour
    {
        public Settings.RoundSetting roundSetting;
        public Settings.HeroList heroList;       // 英雄列表
        public SyncList<int> hasUsedHeros = new SyncList<int>();

        [SerializeField, SyncVar(hook = nameof(OnTimeChange))] int timer;
        void OnTimeChange(int oldTime, int newTime)
        {
            netCanvas.timeTextUI.text = newTime.ToString();
        }

        [SerializeField, SyncVar(hook = nameof(OnCurrentPlayerChange))] int currentPlayerIndex;
        void OnCurrentPlayerChange(int oldPlayer, int newPlayer)
        {

        }

        [SyncVar] public int playerReadyCount = 0;

        TBL.NetCanvas.GameScene netCanvas;
        NetworkRoomManager manager;

        [Header("輪設定")]
        [SyncVar] public int currentRoundPlayerIndex;
        [SyncVar] public bool currentRoundHasSendCard;
        [SyncVar] public int currentRoundSendingCardId;

        private void Start()
        {
            netCanvas = FindObjectOfType<TBL.NetCanvas.GameScene>();
            manager = ((NetworkRoomManager)NetworkManager.singleton);

            StartCoroutine(WaitAllPlayerInit());
        }

        IEnumerator WaitAllPlayerInit()
        {
            while (true)
            {
                int i = 0;
                foreach (NetworkPlayer p in manager.players)
                {
                    if (p.isReady)
                        ++i;
                }

                if (i == manager.roomSlots.Count)
                    break;
                else
                    yield return null;
            }

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

            foreach (NetworkPlayer p in manager.players)
            {
                p.InitPlayer();
            }



            if (isServer)
                StartNewRound(UnityEngine.Random.Range(0, manager.players.Count));
        }

        void StartNewRound(int index)
        {
            currentPlayerIndex = index;
            currentRoundHasSendCard = false;
            currentRoundSendingCardId = 0;

            StartCoroutine(RoundUpdate());
        }

        IEnumerator RoundUpdate()
        {
            manager.players[currentPlayerIndex].TargetDraw();
            yield return StartCoroutine(WaitDraw());

            manager.players[currentPlayerIndex].TargetStartRound();
            float time = roundSetting.roundTime;
            while (!currentRoundHasSendCard && time >= 0)
            {
                time -= Time.deltaTime;
                timer = (int)time;
                yield return null;
            }
        }

        IEnumerator WaitDraw()
        {
            manager.players[currentPlayerIndex].hasDraw = false;
            float time = roundSetting.drawTime;
            while (!manager.players[currentPlayerIndex].hasDraw && time >= 0)
            {
                time -= Time.deltaTime;
                timer = (int)time;
                yield return null;
            }
            if (!manager.players[currentPlayerIndex].hasDraw)
            {
                manager.players[currentPlayerIndex].CmdDrawCard(2);
            }

        }

        public override void OnStartClient()
        {
            base.OnStartClient();

        }
    }

}
