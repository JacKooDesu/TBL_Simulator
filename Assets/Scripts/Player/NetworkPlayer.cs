using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TBL.Card;
using System;

namespace TBL
{
    public class NetworkPlayer : NetworkBehaviour
    {
        public string playerName;

        [SerializeField, Header("手牌")]
        List<CardObject> handCards = new List<CardObject>();

        [SerializeField, Header("情報")]
        List<CardObject> cards = new List<CardObject>();

        [Header("NetLists")]
        public SyncList<int> netHandCard = new SyncList<int>();
        public SyncList<int> netCards = new SyncList<int>();

        private void Start()
        {
            if (isServer)
                ((NetworkRoomManager)NetworkManager.singleton).players.Add(this);

            if (isClient)
                CmdDraw();
        }

        [Command]
        void CmdDraw()
        {
            netHandCard.Add(((NetworkRoomManager)NetworkManager.singleton).deckManager.DrawCardFromTop().ID);
        }


        #region CHAT
        public static event Action<NetworkPlayer, string> OnChatMessage;
        [Command]
        public void CmdChatMessage(string message)
        {
            if (message.Trim() != "")
            {
                RpcChatReceive(message.Trim());
            }
        }

        [ClientRpc]
        public void RpcChatReceive(string message)
        {
            OnChatMessage?.Invoke(this, message);
        }

        #endregion
    }
}

