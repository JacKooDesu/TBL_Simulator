using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TBL.Card;
using TBL.GameActionData;
using System;
using System.Linq;

namespace TBL
{
    public partial class NetworkPlayer : NetworkBehaviour
    {

        public int GetCardCount(params int[] requests)
            => GetCards(requests).Count;
        public List<int> GetCards(params int[] requests)
        {
            List<int> cards = new List<int>();
            if (requests.Length == 0)
                cards.AddRange(netCards);
            else
                cards = netCards.FindAll(id => CardAttributeHelper.Compare(id, requests));

            return cards;
        }

        public int GetHandCardCount(params int[] requests)
            => GetHandCards(requests).Count;
        public List<int> GetHandCards(params int[] requests)
        {
            List<int> cards = new List<int>();
            if (requests.Length == 0)
                cards.AddRange(netHandCards);
            else
                cards = netHandCards.FindAll(id => CardAttributeHelper.Compare(id, requests));

            return cards;
        }

        public List<int> GetOtherPlayers(Func<NetworkPlayer, bool> checker)
        {
            return manager.GetPlayers((p) => p != this && checker(p));
        }
    }
}

