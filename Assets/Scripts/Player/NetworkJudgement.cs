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
            Sending,
            Reacting
        }

        [Header("輪設定")]
        [SyncVar(hook = nameof(OnCurrentPhaseChange))] public Phase currentPhase;
        [SyncVar] Phase lastPhase;
        public void ChangePhase(Phase phase)
        {
            lastPhase = currentPhase;
            currentPhase = phase;
        }
        public void OnCurrentPhaseChange(Phase oldPhase, Phase newPhase)
        {
            if (newPhase == Phase.Reacting || newPhase == Phase.Sending)
            {
                netCanvas.ResetUI();
            }
        }

        [SyncVar] public int currentRoundPlayerIndex;
        [SyncVar] public bool currentRoundHasSendCard;
        [SyncVar] public int currentRoundSendingCardId;
        [SyncVar] public int currentSendingPlayer;
        [SyncVar] public bool currentSendReverse;
        [SyncVar] public int playerIntercept = -1;

        public void ResetRoundTrigger(int hasSendCard = -1, int sendReverse = -1)
        {
            if (hasSendCard != -1)
                currentRoundHasSendCard = hasSendCard == 1;

            if (sendReverse != -1)
                currentSendReverse = sendReverse == 1;
        }

        // server only
        public List<NetworkPlayer> cardSendQueue = new List<NetworkPlayer>();
        public SyncList<int> cardSendQueueID = new SyncList<int>(); // 測試是否可用SyncList

        public List<Action.CardAction> cardActionQueue = new List<Action.CardAction>();
        [SyncVar] public Action.CardAction currentCardAction;   // 用於檢查技能發動

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
            ResetRoundTrigger(0, 0);
            currentPlayerIndex = index;
            currentRoundSendingCardId = 0;
            currentSendingPlayer = -1;
            playerIntercept = -1;

            foreach (NetworkPlayer p in manager.players)
            {
                p.ResetStatus(0, 0, 0, 0, 0);
                // p.isReady = false;
            }

            StartCoroutine(RoundUpdate());
        }

        IEnumerator WaitDraw()
        {
            ChangePhase(Phase.Draw);

            // manager.CheckAllHeroSkill();

            manager.players[currentPlayerIndex].hasDraw = false;
            float time = roundSetting.drawTime;
            while (!manager.players[currentPlayerIndex].hasDraw && time >= 0)
            {
                time -= Time.deltaTime;
                timer = (int)time;

                if (currentPhase == Phase.Reacting)
                    yield return StartCoroutine(CardEventUpdate());

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

            ChangePhase(Phase.ChooseToSend);

            manager.CheckAllHeroSkill();

            manager.players[currentPlayerIndex].TargetRoundStart();
            float time = roundSetting.roundTime;
            while (!currentRoundHasSendCard && time >= 0)
            {
                time -= Time.deltaTime;
                timer = (int)time;

                if (currentPhase == Phase.Reacting)
                    yield return StartCoroutine(CardEventUpdate());

                yield return null;
            }

            if (!currentRoundHasSendCard)
            {
                // remove all hand card
                manager.players[currentPlayerIndex].netHandCard.Clear();
                manager.players[currentPlayerIndex].TargetEndRound();

                StartNewRound(
                (currentPlayerIndex + 1 == manager.players.Count) ?
                0 : currentPlayerIndex + 1
                );
            }
            else
            {
                manager.players[currentPlayerIndex].TargetEndRound();
                StartCoroutine(SendingCardUpdate());
            }
        }

        IEnumerator SendingCardUpdate()
        {
            ChangePhase(Phase.Sending);

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

                    if (playerIntercept != -1)
                    {
                        p.RpcAskCardEnd();
                        p = manager.players[manager.players.FindIndex(x => x.playerIndex == playerIntercept)];
                        p.acceptCard = true;
                        break;
                    }

                    if (currentPhase == Phase.Reacting)
                        yield return StartCoroutine(CardEventUpdate());

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

        public void AddCardAction(Action.CardAction ca)
        {
            if (currentPhase != Phase.Reacting)
            {
                ChangePhase(Phase.Reacting);
                cardActionQueue = new List<Action.CardAction>();
                cardActionQueue.Add(ca);
            }
            else
            {
                cardActionQueue.Add(ca);
            }
        }

        IEnumerator CardEventUpdate()
        {
            int lastActionQueueCount = cardActionQueue.Count;
            float time = roundSetting.reactionTime;
            while (time >= 0)
            {
                time -= Time.deltaTime;
                timer = (int)time;

                if (lastActionQueueCount != cardActionQueue.Count)
                {
                    time = roundSetting.reactionTime;
                    lastActionQueueCount = cardActionQueue.Count;
                }

                yield return null;
            }

            cardActionQueue.Reverse();
            // 處理卡片效果
            for (int i = 0; i < cardActionQueue.Count; ++i)
            {
                if (i >= cardActionQueue.Count)
                    break;

                currentCardAction = cardActionQueue[i];

                Card.CardSetting tempCard = Card.CardSetting.IDConvertCard(cardActionQueue[i].cardId);
                if (tempCard.CardType == Card.CardType.Invalidate)
                {
                    Card.CardSetting nextCard = Card.CardSetting.IDConvertCard(cardActionQueue[i + 1].cardId);
                    i += 1;

                    print($"{tempCard.GetCardNameFully()} 無效化 {nextCard.GetCardNameFully()}");
                    continue;
                }
                else
                {
                    manager.DeckManager.Deck.GetCardPrototype(cardActionQueue[i].cardId).OnEffect(manager, cardActionQueue[i]);
                    print($"{tempCard.CardName} 效果發動");

                    manager.CheckAllHeroSkill();     

                    time = roundSetting.reactionTime;
                    while (time >= 0)
                    {
                        time -= Time.deltaTime;
                        timer = (int)time;

                        yield return null;
                    }
                }
            }

            ChangePhase(lastPhase);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
        }
    }

}
