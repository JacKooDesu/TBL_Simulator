using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TBL.Card;
using TBL.GameAction;
using System;

namespace TBL
{
    public partial class NetworkPlayer : NetworkBehaviour
    {
        [Header("狀態")]
        [SyncVar] public bool isReady=false;
        [Command] public void CmdSetReady(bool b) { isReady = b; }
        // [SyncVar] public bool isReadyLast;

        [SyncVar(hook = nameof(OnLockStatusChange))] public bool isLocked;
        void OnLockStatusChange(bool oldStatus, bool newStatus) { netCanvas.playerUIs[playerIndex].UpdateStatus(); }
        [Command] public void CmdSetLocked(bool b) { isLocked = b; }
        // [SyncVar] public bool isLockedLast;
        [SyncVar(hook = nameof(OnSkipStatusChange))] public bool isSkipped;
        void OnSkipStatusChange(bool oldStatus, bool newStatus) { netCanvas.playerUIs[playerIndex].UpdateStatus(); }
        [Command] public void CmdSetSkipped(bool b) { isSkipped = b; }

        [SyncVar] public bool hasDraw;
        [Command] public void CmdSetDraw(bool b) { hasDraw = b; }

        [SyncVar] public bool acceptCard;
        [Command] public void CmdSetAcceptCard(bool b) { acceptCard = b; }
        [SyncVar] public bool rejectCard;
        [Command] public void CmdSetRejectCard(bool b) { rejectCard = b; }

        public void ResetStatus(int isLocked = -1, int isSkipped = -1, int hasDraw = -1, int acceptCard = -1, int rejectCard = -1)
        {
            if (isLocked != -1)
                this.isLocked = isLocked == 1;

            if (isSkipped != -1)
                this.isSkipped = isSkipped == 1;

            if (hasDraw != -1)
                this.hasDraw = hasDraw == 1;

            if (acceptCard != -1)
                this.acceptCard = acceptCard == 1;

            if (rejectCard != -1)
                this.rejectCard = rejectCard == 1;
        }

        [SyncVar(hook = nameof(OnWinStatusChange))] public bool isWin = false;
        void OnWinStatusChange(bool oldStatus, bool newStatus)
        {
            if (newStatus == true)
            {
                manager.Judgement.PlayerWin(this);
            }
        }

        [SyncVar(hook = nameof(OnDeadStatusChange))] public bool isDead = false;
        void OnDeadStatusChange(bool oldStatus, bool newStatus)
        {
            if (newStatus == true)
            {
                manager.RpcLog(UI.LogSystem.LogGeneral.PlayerDead(this), this);
            }
        }
    }
}

