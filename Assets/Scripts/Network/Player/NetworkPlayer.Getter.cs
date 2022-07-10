using System.Collections;
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
        public int GetCardColorCount(CardColor color)
        {
            int result = 0;
            foreach (var c in netCards)
            {
                if (((CardSetting)c).CardColor == color)
                    result++;
            }

            return result;
        }


        public int GetHandCardColorCount(CardColor color)
        {
            int result = 0;
            foreach (var c in netHandCards)
            {
                if (((CardSetting)c).CardColor == color)
                    result++;
            }

            return result;
        }

        public int GetHandCardTypeCount(CardType cardType)
        {
            int result = 0;
            foreach (var c in netHandCards)
            {
                if (((CardSetting)c).CardType == cardType)
                    result++;
            }

            return result;
        }

        public int GetHandCardSendTypeCount(CardSendType sendType)
        {
            int result = 0;
            foreach (var c in netHandCards)
            {
                if (((CardSetting)c).SendType == sendType)
                    result++;
            }

            return result;
        }
    }
}

