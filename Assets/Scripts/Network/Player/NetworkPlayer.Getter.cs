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
        public int GetCardCount(params CardColor[] colors)
            => GetCards(colors).Count;

        public int GetCardCount(params CardType[] cardTypes)
            => GetCards(cardTypes).Count;

        public int GetCardCount(params CardSendType[] sendTypes)
            => GetCards(sendTypes).Count;

        public List<int> GetCards(params CardColor[] colors)
        {
            List<int> cards = new List<int>();
            List<CardColor> colorList = new List<CardColor>(colors);
            if (colors.Length == 0)
            {
                colorList.Add(CardColor.Black);
                colorList.Add(CardColor.Red);
                colorList.Add(CardColor.Blue);
            }

            foreach (var c in netCards)
            {
                if (colorList.Contains(((CardSetting)c).CardColor))
                    cards.Add(c);
            }

            return cards;
        }

        public List<int> GetCards(params CardType[] colors)
        {
            List<int> cards = new List<int>();
            List<CardType> typeList = new List<CardType>(colors);
            if (colors.Length == 0)
                typeList = Enum.GetValues(typeof(CardType)).Cast<CardType>().ToList();

            foreach (var c in netCards)
            {
                if (typeList.Contains(((CardSetting)c).CardType))
                    cards.Add(c);
            }

            return cards;
        }

        public List<int> GetCards(params CardSendType[] colors)
        {
            List<int> cards = new List<int>();
            List<CardSendType> sendTypeList = new List<CardSendType>(colors);
            if (colors.Length == 0)
            {
                sendTypeList.Add(CardSendType.Direct);
                sendTypeList.Add(CardSendType.Secret);
                sendTypeList.Add(CardSendType.Public);
            }

            foreach (var c in netCards)
            {
                if (sendTypeList.Contains(((CardSetting)c).SendType))
                    cards.Add(c);
            }

            return cards;
        }


        public int GetHandCardCount(params CardColor[] colors)
            => GetHandCards(colors).Count;

        public int GetHandCardCount(params CardType[] cardTypes)
            => GetHandCards(cardTypes).Count;

        public int GetHandCardCount(params CardSendType[] sendTypes)
            => GetHandCards(sendTypes).Count;

        public List<int> GetHandCards(params CardColor[] colors)
        {
            List<int> cards = new List<int>();
            List<CardColor> colorList = new List<CardColor>(colors);
            if (colors.Length == 0)
            {
                colorList.Add(CardColor.Black);
                colorList.Add(CardColor.Red);
                colorList.Add(CardColor.Blue);
            }

            foreach (var c in netHandCards)
            {
                if (colorList.Contains(((CardSetting)c).CardColor))
                    cards.Add(c);
            }

            return cards;
        }

        public List<int> GetHandCards(params CardType[] colors)
        {
            List<int> cards = new List<int>();
            List<CardType> typeList = new List<CardType>(colors);
            if (colors.Length == 0)
                typeList = Enum.GetValues(typeof(CardType)).Cast<CardType>().ToList();

            foreach (var c in netHandCards)
            {
                if (typeList.Contains(((CardSetting)c).CardType))
                    cards.Add(c);
            }

            return cards;
        }

        public List<int> GetHandCards(params CardSendType[] colors)
        {
            List<int> cards = new List<int>();
            List<CardSendType> sendTypeList = new List<CardSendType>(colors);
            if (colors.Length == 0)
            {
                sendTypeList.Add(CardSendType.Direct);
                sendTypeList.Add(CardSendType.Secret);
                sendTypeList.Add(CardSendType.Public);
            }

            foreach (var c in netHandCards)
            {
                if (sendTypeList.Contains(((CardSetting)c).SendType))
                    cards.Add(c);
            }

            return cards;
        }
    }
}

