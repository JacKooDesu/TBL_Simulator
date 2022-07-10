﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TBL.Card;
using TBL.Action;
using System;

namespace TBL
{
    public partial class NetworkPlayer : NetworkBehaviour
    {
        [Server]
        public void DrawTeam()
        {
            teamIndex = ((NetworkRoomManager)NetworkManager.singleton).teamList[0];
            ((NetworkRoomManager)NetworkManager.singleton).teamList.RemoveAt(0);
        }
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
        public void AddCard(int id)
        {
            print($"檯面新增 {id}");
            netCards.Add((int)id);
        }

        [Server]
        public void DrawHero()
        {
            int rand;
            do
            {
                rand = UnityEngine.Random.Range(0, manager.Judgement.heroList.heros.Count);
            } while (manager.Judgement.hasUsedHeros.IndexOf(rand) != -1);

            manager.Judgement.hasUsedHeros.Add(rand);
            heroIndex = rand;
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
        [Command]
        public void CmdDrawCard(int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                netHandCards.Add(manager.DeckManager.DrawCardFromTop().ID);
            }
        }

        [Server]
        public void SetName()
        {
            playerName = GameUtils.PlayerName;
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
        [Server]
        public void AddHandCard(int id)
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
    }
}

