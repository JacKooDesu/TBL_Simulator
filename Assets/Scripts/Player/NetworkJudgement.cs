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

        [SerializeField, SyncVar(hook = nameof(OnCurrentPlayerChange))] public int currentPlayerIndex;
        void OnCurrentPlayerChange(int oldPlayer, int newPlayer)
        {

        }

        [SyncVar] public int playerReadyCount = 0;

        TBL.NetCanvas.GameScene netCanvas;
        NetworkRoomManager manager;

        // 階段
        public enum Phase
        {
            Draw,
            ChooseToSend,
            Sending
        }

        [Header("輪設定")]
        [SyncVar] public Phase currentPhase;
        [SyncVar] Phase lastPhase;
        [SyncVar] public int currentRoundPlayerIndex;
        [SyncVar] public bool currentRoundHasSendCard;
        [SyncVar] public int currentRoundSendingCardId;
        [SyncVar] public int currentSendingPlayer;
        [SyncVar] public bool currentSendReverse;

        // server only
        public List<NetworkPlayer> cardSendQueue = new List<NetworkPlayer>();
        public SyncList<int> cardSendQueueID = new SyncList<int>(); // 測試是否可用SyncList

        public List<Card.CardSetting> cardActionQueue = new List<Card.CardSetting>();

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
            currentSendReverse = false;
            currentSendingPlayer = -1;

            foreach (NetworkPlayer p in manager.players)
            {
                p.hasDraw = (false);
                // p.isReady = (false);
                p.rejectCard = (false);
                p.acceptCard = (false);
                p.isLocked = (false);
                p.isSkipped = (false);
            }

            StartCoroutine(RoundUpdate());
        }

        IEnumerator WaitDraw()
        {
            currentPhase = Phase.Draw;

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
                manager.players[currentPlayerIndex].DrawCard(2);
            }
        }

        IEnumerator RoundUpdate()
        {
            manager.players[currentPlayerIndex].RpcUpdateHostPlayer();

            manager.players[currentPlayerIndex].TargetDrawStart();
            yield return StartCoroutine(WaitDraw());

            currentPhase = Phase.ChooseToSend;

            manager.players[currentPlayerIndex].TargetRoundStart();
            float time = roundSetting.roundTime;
            while (!currentRoundHasSendCard && time >= 0)
            {
                time -= Time.deltaTime;
                timer = (int)time;
                yield return null;
            }

            if (!currentRoundHasSendCard)
            {
                // remove all hand card
                manager.players[currentPlayerIndex].netHandCard.Clear();

                StartNewRound(
                (currentPlayerIndex + 1 == manager.players.Count) ?
                0 : currentPlayerIndex + 1
                );
            }
            else
            {
                StartCoroutine(SendingCardUpdate());
            }
        }

        IEnumerator SendingCardUpdate()
        {
            currentPhase = Phase.Sending;

            print("卡片傳送中");
            int iter = 1;
            // foreach (NetworkPlayer p in cardSendQueue)
            for (int i = 0; i < cardSendQueue.Count; i += iter)
            {
                if (i <= -1)
                {
                    i = cardSendQueue.Count - 1;
                }
                NetworkPlayer p = cardSendQueue[i];
                currentSendingPlayer = p.playerIndex;

                if (p.playerIndex == currentPlayerIndex)
                {
                    p.AddCard(currentRoundSendingCardId);
                    break;
                }

                if (p.isLocked)
                {
                    p.AddCard(currentRoundSendingCardId);
                    break;
                }

                p.RpcAskCardStart();
                float time = roundSetting.reactionTime;
                while (!p.rejectCard && !p.acceptCard && time >= 0)
                {
                    time -= Time.deltaTime;
                    timer = (int)time;

                    if (currentSendReverse)
                        iter = -1;

                    yield return null;
                }

                if (!p.rejectCard && !p.acceptCard)
                    p.rejectCard = true;

                p.RpcAskCardEnd();

                if (p.rejectCard)
                    continue;
                if (p.acceptCard)
                {
                    p.AddCard(currentRoundSendingCardId);
                    break;
                }

            }

            StartNewRound(
                (currentPlayerIndex + 1 == manager.players.Count) ?
                0 : currentPlayerIndex + 1
                );
        }

        IEnumerator CardEventUpdate()
        {
            float time = roundSetting.reactionTime;
            while (time >= 0)
            {
                time -= Time.deltaTime;
                timer = (int)time;
                yield return null;
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

        }
    }

}
