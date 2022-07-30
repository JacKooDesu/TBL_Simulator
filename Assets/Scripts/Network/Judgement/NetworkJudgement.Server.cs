using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

namespace TBL
{
    using UI.LogSystem;
    public partial class NetworkJudgement : NetworkBehaviour
    {
        public List<NetworkPlayer> cardSendQueue = new List<NetworkPlayer>();
        public SyncList<int> cardSendQueueID = new SyncList<int>(); // 測試是否可用SyncList

        public List<GameAction.CardAction> cardActionQueue = new List<GameAction.CardAction>();
        [SyncVar] public GameAction.CardAction currentCardAction;   // 用於檢查技能發動

        IEnumerator WaitAllPlayerInit()
        {
            yield return new WaitUntil(
                () => manager.players.Find(player => !player.isReady) == null);

            InitPlayer();

            StartNewRound(UnityEngine.Random.Range(0, manager.players.Count));
        }

        void InitPlayer()
        {
            foreach (NetworkPlayer p in manager.players)
            {
                p.DrawTeam();
                p.DrawHero(7);
                p.DrawCard(3);
            }
        }

        void StartNewRound(int index)
        {
            ResetRoundTrigger(0, 0);
            currentRoundPlayerIndex = index;
            currentRoundSendingCardId = 0;
            currentSendingPlayer = -1;
            playerIntercept = -1;

            currentCardAction = new GameAction.CardAction(-1, -1, 0, 0, 0);

            foreach (NetworkPlayer p in manager.players)
            {
                p.ResetStatus(0, 0, 0, 0, 0);

                for (int i = 0; i < p.netHeroSkillCanActivate.Count; ++i)
                    p.CmdSetSkillCanActivate(i, false);
            }

            if (currentPhase == Phase.Result)
                return;

            var player = manager.players[currentRoundPlayerIndex];
            manager.RpcLog(LogGeneral.RoundStart(player), player);

            StartCoroutine(RoundUpdate());
        }

        IEnumerator WaitDraw(int amount)
        {
            ChangePhase(Phase.Draw);

            // manager.CheckAllHeroSkill();

            manager.players[currentRoundPlayerIndex].hasDraw = false;
            float time = roundSetting.drawTime;
            while (!manager.players[currentRoundPlayerIndex].hasDraw && time >= 0)
            {
                time -= Time.deltaTime;
                timer = (int)time;

                if (currentPhase == Phase.Reacting)
                    yield return StartCoroutine(CardEventUpdate());

                yield return null;
            }

            manager.players[currentRoundPlayerIndex].DrawCard(amount);
        }

        IEnumerator RoundUpdate()
        {
            var player = manager.players[currentRoundPlayerIndex];
            player.RpcUpdateRoundHost();

            player.TargetDrawStart();
            yield return StartCoroutine(WaitDraw(2));

            ChangePhase(Phase.ChooseToSend);

            manager.CheckAllHeroSkill();

            player.TargetRoundStart();
            float time = roundSetting.roundTime;
            while (!currentRoundHasSendCard && time >= 0)
            {
                time -= Time.deltaTime;
                timer = (int)time;

                if (currentPhase == Phase.Reacting)
                {
                    yield return StartCoroutine(CardEventUpdate());
                }

                if (currentPhase == Phase.HeroSkillReacting)
                {
                    yield return StartCoroutine(HeroSkillReactingUpdate());
                    time = roundSetting.roundTime;  // reset time after hero skill activate
                }

                yield return null;
            }

            if (!currentRoundHasSendCard)
            {
                // remove all hand card
                player.netHandCards.Clear();
                player.TargetEndRound();
                manager.RpcLog(UI.LogSystem.LogGeneral.RoundTimeOver(player), player);
                StartNewRound(
                    (currentRoundPlayerIndex + 1 == manager.players.Count) ?
                    0 : currentRoundPlayerIndex + 1
                );
            }
            else
            {
                player.TargetEndRound();
                yield return StartCoroutine(SendingCardUpdate());
                if (currentPhase == Phase.HeroSkillReacting)
                    yield return StartCoroutine(HeroSkillReactingUpdate());

                StartNewRound(
                    (currentRoundPlayerIndex + 1 == manager.players.Count) ?
                    0 : currentRoundPlayerIndex + 1
                );
            }
        }

        IEnumerator SendingCardUpdate()
        {
            ChangePhase(Phase.Sending);

            var player = manager.players[currentRoundPlayerIndex];

            print("卡片傳送中");
            manager.RpcLog(UI.LogSystem.LogGeneral.SendCard(player, (Card.CardSetting)currentRoundSendingCardId), player);

            int iter = 1;
            // foreach (NetworkPlayer p in cardSendQueue)
            for (int i = 0; i < cardSendQueue.Count; i += iter)
            {
                // 頭尾
                if (i <= -1)
                    i = cardSendQueue.Count - 1;

                NetworkPlayer p = cardSendQueue[i];
                currentSendingPlayer = p.playerIndex;

                if (currentSendingPlayer == currentRoundPlayerIndex)
                {
                    manager.RpcLog(UI.LogSystem.LogGeneral.AcceptCard(p, (Card.CardSetting)currentRoundSendingCardId), p);
                    p.AddCard(currentRoundSendingCardId);
                    p.acceptCard = true;
                    break;
                }

                if (p.isLocked)
                {
                    manager.RpcLog(UI.LogSystem.LogGeneral.AcceptCard(p, (Card.CardSetting)currentRoundSendingCardId), p);
                    p.AddCard(currentRoundSendingCardId);
                    p.acceptCard = true;
                    break;
                }

                p.RpcAskCardStart(currentRoundSendingCardId);
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
                        p = manager.players[playerIntercept];
                        p.acceptCard = true;
                        break;
                    }

                    if (currentPhase == Phase.Reacting)
                    {
                        yield return StartCoroutine(CardEventUpdate());
                        manager.CheckAllHeroSkill();
                    }

                    yield return null;
                }

                if (!p.rejectCard && !p.acceptCard)
                    p.rejectCard = true;

                p.RpcAskCardEnd();

                if (p.rejectCard)
                    continue;
                if (p.acceptCard)
                {
                    manager.RpcLog(UI.LogSystem.LogGeneral.AcceptCard(p, (Card.CardSetting)currentRoundSendingCardId), p);
                    p.AddCard(currentRoundSendingCardId);
                    break;
                }

            }

            manager.CheckAllWin();
            manager.CheckAllHeroSkill();
        }

        public void AddCardAction(GameAction.CardAction ca)
        {
            if (currentPhase != Phase.Reacting)
            {
                ChangePhase(Phase.Reacting);
                cardActionQueue = new List<GameAction.CardAction>();
            }
            cardActionQueue.Add(ca);

            manager.CheckAllHeroSkill();

            manager.TargetLogAll(UI.LogSystem.LogGeneral.UseCard(ca));
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

                var lastAction = cardActionQueue[cardActionQueue.Count - 1];
                // 鋼鐵特JK的技能當作識破使用，但又不可被識破，故強制退出迴圈
                if (((Card.CardSetting)lastAction.cardId).CardType == Card.CardType.Invalidate &&
                    lastAction.suffix == 1)
                {
                    timer = 0;
                    cardActionQueue.Clear();
                    break;
                }

                yield return null;
            }

            manager.CheckAllHeroSkill();

            cardActionQueue.Reverse();
            // 處理卡片效果
            for (int i = 0; i < cardActionQueue.Count; ++i)
            {
                if (i >= cardActionQueue.Count)
                    break;

                currentCardAction = cardActionQueue[i];

                Card.CardSetting tempCard = Card.CardSetting.IdToCard(cardActionQueue[i].cardId);
                if (tempCard.CardType == Card.CardType.Invalidate)
                {
                    Card.CardSetting nextCard = Card.CardSetting.IdToCard(cardActionQueue[i + 1].cardId);
                    i += 1;

                    print($"{tempCard.GetCardNameFully()} 無效化 {nextCard.GetCardNameFully()}");
                }
                else
                {
                    manager.DeckManager.Deck.GetCardPrototype(cardActionQueue[i].cardId).OnEffect(manager, cardActionQueue[i]);
                    print($"{tempCard.CardName} 效果發動");
                }

                manager.CheckAllHeroSkill();

                time = roundSetting.reactionTime;
                while (time >= 0)
                {
                    time -= Time.deltaTime;
                    timer = (int)time;

                    yield return null;
                }
            }

            ChangePhase(lastPhase);

            manager.CheckAllHeroSkill();
        }

        IEnumerator HeroSkillReactingUpdate()
        {
            float time = roundSetting.reactionTime;

            while (time >= 0)
            {
                if (currentPhase != Phase.HeroSkillReacting)
                    yield break;

                time -= Time.deltaTime;
                timer = ((int)time);

                yield return null;
            }
            ChangePhase(lastPhase);
        }

        public void PlayerWin(NetworkPlayer p)
        {
            if (currentPhase == Phase.Result)
                return;

            ChangePhase(Phase.Result);

            manager.TargetLogAll(UI.LogSystem.LogGeneral.PlayerWin(p));
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
        }
    }

}
