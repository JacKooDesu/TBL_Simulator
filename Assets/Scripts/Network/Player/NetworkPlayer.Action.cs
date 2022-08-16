using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TBL.Card;
using TBL.GameActionData;
using System.Threading.Tasks;

// Action means only run on server (not use for client command)
namespace TBL
{
    using Util;
    public partial class NetworkPlayer : NetworkBehaviour
    {
        [Server]
        public void DrawTeam()
        {
            teamIndex = ((NetworkRoomManager)NetworkManager.singleton).teamList[0];
            ((NetworkRoomManager)NetworkManager.singleton).teamList.RemoveAt(0);
        }

        [Server]
        public void AddCard(int id)
        {
            print($"System - 檯面新增 {id}");
            netCards.Add((int)id);
        }

        [Server]
        public void DrawHero(int testIndex = -1, bool serverOnly = false)
        {
            if (testIndex != -1)
            {
                heroIndex = testIndex;
                return;
            }

            int rand;
            do
            {
                rand = UnityEngine.Random.Range(0, manager.Judgement.heroList.heros.Count);
            } while (manager.Judgement.hasUsedHeros.IndexOf(rand) != -1);

            manager.Judgement.hasUsedHeros.Add(rand);
            heroIndex = rand;
        }

        [Server]
        public void DrawCard(int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                netHandCards.Add(manager.DeckManager.DrawCardFromTop().ID);
            }

            if (manager.Judgement.currentRoundPlayerIndex == playerIndex)
                CmdSetDraw(true);
        }

        [Server]
        public void SetName()
        {
            playerName = GameUtils.PlayerName;
        }

        [Server]
        public void AddHandCard(int id)
        {
            print($"手牌新增 {id}");
            netHandCards.Add((int)id);
        }

        [Server]
        public void SetHeroState(bool hiding)
        {
            hero.isHiding = hiding;
            RpcChangeHeroState(hiding);
        }

        [Server]
        public void SetSkillCanActivate(int index, bool b)
        {
            netHeroSkillCanActivate[index] = b;
        }

        [Server]
        public void SetSkillLimited(int index, bool b)
        {
            hero.skills[index].limited = b;
        }

        [Server]
        public void SetSkillRoundLimited(int index, bool b)
        {
            hero.skills[index].roundLimited = b;
        }

        // H = Hand
        // T = Table
        // G = Graveyard
        // D = Deck

        [Server]
        public void CardHToH(int id, int target)
        {
            print($"玩家 {playerIndex} 給予 玩家 {target} - {Card.CardSetting.IdToCard(id).name}");
            netHandCards.Remove((int)id);
            manager.players[target].AddHandCard(id);
        }

        [Server]
        public void CardHToT(int id, int target)     // Hand to Table
        {
            print($"玩家 {playerIndex} 給予 玩家 {target} - {Card.CardSetting.IdToCard(id).name}");
            netHandCards.Remove((int)id);
            manager.players[target].AddCard(id);
        }

        [Server]
        public void CardHToG(int id) // Hand To Graveyard
        {
            netHandCards.Remove((int)id);
        }

        [Server]
        public void CardTToG(int player, int id) // Table ToGraveyard
        {
            netCards.Remove(id);
        }

        [Server]
        public void CardTToH(int player, int id)  // Table to Hand
        {
            manager.players[player].netCards.Remove(id);
            manager.players[player].netHandCards.Add(id);
        }

        [Server]
        public void CardHToD(int id)     // Hand to Deck
        {
            manager.DeckManager.CardToTop(id);
        }

        [Server]
        public void SetWaitingData(bool value) => isWaitingData = value;

        [Server]
        public void SetTempData(int data)
        {
            tempData = data;
            CmdSetWaitingData(false);
        }

        [Server]
        public void ClearTempData() => tempData = int.MinValue;

        [Server]
        public async Task InitReturnDataMenu(params string[] optionTexts)
        {
            this.TargetReturnDataMenu(optionTexts);
            await TaskExtend.WaitUntil(() => isWaitingData);
            return;
        }

        [Server]
        public async Task InitReturnHandCardMenu(int playerIndex, params int[] requests)
        {
            this.TargetReturnHandCardMenu(playerIndex, requests);
            await TaskExtend.WaitUntil(() => isWaitingData);
            return;
        }

        [Server]
        public async Task InitReturnCardMenu(int playerIndex, params int[] requests)
        {
            this.TargetReturnCardMenu(playerIndex, requests);
            await TaskExtend.WaitUntil(() => isWaitingData);
            return;
        }

        [Server]
        public async Task ReturnPlayer(List<int> targets)
        {
            this.TargetReturnPlayer(targets);
            await TaskExtend.WaitUntil(() => isWaitingData);
            return;
        }

        [Server]
        public async void GetTest(bool isDraw)
        {
            string msg = $"玩家 {playerIndex} ({playerName}) ：";
            string actionStr = (isDraw ? "抽一張牌" : "我是一個好人");

            await InitReturnDataMenu(actionStr);
            await TaskExtend.WaitUntil(
                () => !isWaitingData,
                () => judgement.currentPhase != NetworkJudgement.Phase.Reacting
            );

            if (judgement.currentPhase != NetworkJudgement.Phase.Reacting || tempData == int.MinValue || tempData == 0)
            {
                if (isDraw)
                    DrawCard(1);
            }
            else
            {
                if (!isDraw)
                {
                    DrawCard(1);
                    actionStr = (!isDraw ? "抽一張牌" : "我是一個好人");
                }
            }
            RpcAddLog(
                msg + actionStr,
                true,
                false,
                new int[] { }
            );
        }
    }
}

