using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TBL.Card;
using TBL.GameAction;
using System;
using System.Linq;

namespace TBL
{
    public partial class NetworkPlayer : NetworkBehaviour
    {
        [Command]
        public void CmdDrawTeam()
        {
            teamIndex = ((NetworkRoomManager)NetworkManager.singleton).teamList[0];
            ((NetworkRoomManager)NetworkManager.singleton).teamList.RemoveAt(0);
        }

        [Command]
        public void CmdAddCard(int id)
        {
            print($"檯面新增 {id}");
            netCards.Add((int)id);
        }

        [Command]
        public void CmdDrawHero()
        {
            // -- 用於測試固定英雄 --
            if (isLocalPlayer)
            {
                heroIndex = 8;
                return;
            }
            // --

            int rand;
            do
            {
                rand = UnityEngine.Random.Range(0, manager.Judgement.heroList.heros.Count);
            } while (manager.Judgement.hasUsedHeros.IndexOf(rand) != -1);

            manager.Judgement.hasUsedHeros.Add(rand);
            heroIndex = rand;
        }

        [Command]
        public void CmdDrawCard(int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                netHandCards.Add(manager.DeckManager.DrawCardFromTop().ID);
            }
        }

        [Command]
        public void CmdSetName()
        {
            playerName = GameUtils.PlayerName;
        }

        [Command]
        public void CmdAddHandCard(int id)
        {
            print($"手牌新增 {id}");
            netHandCards.Add((int)id);
        }

        [Command]
        public void CmdCardHToH(int id, int target)     // Hand To Hand
        {
            print($"玩家 {playerIndex} 給予 玩家 {target} - {Card.CardSetting.IdToCard(id).name}");
            netHandCards.Remove((int)id);
            manager.players[target].AddHandCard(id);
        }

        [Command]
        public void CmdCardHToT(int id, int target)     // Hand to Table
        {
            print($"玩家 {playerIndex} 給予 玩家 {target} - {Card.CardSetting.IdToCard(id).name}");
            netHandCards.Remove((int)id);
            manager.players[target].AddCard(id);
        }

        [Command]
        public void CmdCardHToG(int id) // Hand To Graveyard
        {
            netHandCards.Remove((int)id);
        }

        [Command]
        public void CmdCardTToG(int player, int id) // Table ToGraveyard
        {
            manager.players[player].netCards.Remove(id);
        }

        [Command]
        public void CmdCardTToH(int player, int id)  // Table to Hand
        {
            manager.players[player].netCards.Remove(id);
            manager.players[player].netHandCards.Add(id);
        }

        [Command]
        public void CmdCardHToD(int id)     // Hand to Deck
        {
            manager.DeckManager.CardToTop(id);
        }

        [Command]
        public void CmdSetWaitingData(bool value) => isWaitingData = value;

        [Command]
        public void CmdSetTempData(int data)
        {
            tempData = data;
            CmdSetWaitingData(false);
        }

        [Command]
        public void CmdClearTempData() => tempData = int.MinValue;

        [TargetRpc]
        public void TargetReturnDataMenu(params string[] optionTexts)
        {
            print("Target data return menu init");
            var options = new List<Option>();
            for (int i = 0; i < optionTexts.Length; ++i)
            {
                int temp = i;
                options.Add(
                    new Option
                    {
                        str = optionTexts[temp],
                        onSelect = () => CmdSetTempData(temp)
                    }
                );
            }
            netCanvas.InitMenu(options);
            CmdSetWaitingData(true);
        }

        [TargetRpc]
        public void TargetReturnHandCardMenu(int playerIndex, params int[] requests)
        {
            var targetHandCards = manager.players[playerIndex].netHandCards;
            var options = new List<Option>();
            for (int i = 0; i < targetHandCards.Count; ++i)
            {
                var card = targetHandCards[i];
                if (!CardAttributeHelper.Compare(card, requests))
                    continue;

                options.Add(
                    new Option
                    {
                        str = ((CardSetting)card).CardName,
                        onSelect = () => CmdSetTempData(card)
                    }
                );
            }
            netCanvas.InitMenu(options);
            CmdSetWaitingData(true);
        }

        [TargetRpc]
        public void TargetReturnCardMenu(int playerIndex, params int[] requests)
        {
            var targetCards = manager.players[playerIndex].netCards;
            var options = new List<Option>();
            for (int i = 0; i < targetCards.Count; ++i)
            {
                var card = targetCards[i];
                if (!CardAttributeHelper.Compare(card, requests))
                    continue;

                options.Add(
                    new Option
                    {
                        str = ((CardSetting)card).CardName,
                        onSelect = () => CmdSetTempData(card)
                    }
                );
            }
            netCanvas.InitMenu(options);
            CmdSetWaitingData(true);
        }
    }
}

