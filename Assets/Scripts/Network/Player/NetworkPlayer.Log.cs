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
        [Command]
        public void CmdAddLog(string message, bool isServer, bool isPrivate, int[] targetPlayers)
        {
            RpcAddLog(message, isServer, isPrivate, targetPlayers);
        }

        [ClientRpc]
        public void RpcAddLog(string message, bool isServer, bool isPrivate, int[] targetPlayers)
        {
            var log = new UI.LogSystem.LogBase(message, isServer, isPrivate, targetPlayers);
            UI.LogSystem.LogBase.logs.Add(log);

            List<int> targetList = new List<int>();
            if (log.TargetPlayers.Length == 0)
            {
                for (int i = 0; i < manager.players.Count; ++i)
                {
                    int x = i;
                    targetList.Add(x);
                }
            }
            else
            {
                targetList.AddRange(log.TargetPlayers);
            }

            if (targetList.IndexOf(manager.LocalRoomPlayerIndex) != -1)
                netCanvas.AddLog(UI.LogSystem.LogBase.logs.Count - 1);
        }

        [TargetRpc]
        public void TargetAddLog(string message, bool isServer, bool isPrivate, int[] targetPlayers, bool canvasLog)
        {
            var log = new UI.LogSystem.LogBase(message, isServer, isPrivate, targetPlayers);
            UI.LogSystem.LogBase.logs.Add(log);

            if (canvasLog)
                netCanvas.AddLog(UI.LogSystem.LogBase.logs.Count - 1);
        }

        public void AddLog(string message)
        {
            var log = new UI.LogSystem.LogBase(message, false, true, new int[] { playerIndex });
            UI.LogSystem.LogBase.logs.Add(log);

            netCanvas.AddLog(UI.LogSystem.LogBase.logs.Count - 1);
        }
    }
}

