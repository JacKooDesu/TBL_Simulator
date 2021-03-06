using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TBL.Card;
using TBL.GameAction;
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
            print($"檯面新增 {id}");
            netCards.Add((int)id);
        }

        [Server]
        public void DrawHero(int testIndex = -1)
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

            if (manager.Judgement.currentPlayerIndex == playerIndex)
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
            manager.players[player].netCards.Remove(id);
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
    }
}

