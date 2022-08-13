using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TBL.Card;
using TBL.GameActionData;
using System;

namespace TBL
{
    public partial class NetworkPlayer : NetworkBehaviour
    {
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

        [Command]
        public void CmdUpdatePlayerStatusUI()
        {
            for (int i = 0; i < manager.players.Count; ++i)
                RpcUpdatePlayerStatusUI(i);
        }

        [ClientRpc]
        public void RpcUpdatePlayerStatusUI(int i)
        {
            netCanvas.playerUIs[i].UpdateStatus();
        }
    }
}

