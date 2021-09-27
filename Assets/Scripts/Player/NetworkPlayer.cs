﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TBL.Card;
using System;

namespace TBL
{
    public class NetworkPlayer : NetworkBehaviour
    {
        [SyncVar]
        public int playerIndex = 0;

        [Command]
        void CmdSetPlayerIndex(int i)
        {
            playerIndex = i;
        }

        [SyncVar(hook = nameof(OnPlayerNameChange))]
        public string playerName;
        void OnPlayerNameChange(string oldName, string newName)
        {

        }

        [SerializeField, Header("手牌")]
        List<CardObject> handCards = new List<CardObject>();

        [SerializeField, Header("情報")]
        List<CardObject> cards = new List<CardObject>();

        [Header("NetLists")]
        public SyncList<int> netHandCard = new SyncList<int>();
        // net hand card callback
        public void OnUpdateHandCard(SyncList<int>.Operation op, int index, int oldItem, int newItem)
        {
            switch (op)
            {
                case SyncList<int>.Operation.OP_ADD:
                    // index is where it got added in the list
                    // newItem is the new item
                    if (isLocalPlayer)
                    {
                        netCanvas.UpdateCardList();
                    }

                    netCanvas.playerUIs[manager.GetPlayerSlotIndex(this)].handCardCount.text = netHandCard.Count.ToString();
                    break;
                case SyncList<int>.Operation.OP_CLEAR:
                    if (isLocalPlayer)
                    {
                        netCanvas.UpdateCardList();
                    }

                    netCanvas.playerUIs[manager.GetPlayerSlotIndex(this)].handCardCount.text = netHandCard.Count.ToString();
                    // list got cleared
                    break;
                case SyncList<int>.Operation.OP_INSERT:
                    // index is where it got added in the list
                    // newItem is the new item
                    break;
                case SyncList<int>.Operation.OP_REMOVEAT:
                    // index is where it got removed in the list
                    // oldItem is the item that was removed
                    if (isLocalPlayer)
                    {
                        netCanvas.UpdateCardList();
                    }

                    netCanvas.playerUIs[manager.GetPlayerSlotIndex(this)].handCardCount.text = netHandCard.Count.ToString();
                    break;
                case SyncList<int>.Operation.OP_SET:
                    // index is the index of the item that was updated
                    // oldItem is the previous value for the item at the index
                    // newItem is the new value for the item at the index
                    break;
            }
        }

        public SyncList<int> netCards = new SyncList<int>();
        // net hand card callback
        public void OnUpdateCard(SyncList<int>.Operation op, int index, int oldItem, int newItem)
        {
            switch (op)
            {
                case SyncList<int>.Operation.OP_ADD:
                    // index is where it got added in the list
                    // newItem is the new item
                    if (isLocalPlayer)
                    {
                        // netCanvas.UpdateCardList();
                    }
                    netCanvas.playerUIs[playerIndex].UpdateStatus();
                    break;
                case SyncList<int>.Operation.OP_CLEAR:
                    // list got cleared
                    break;
                case SyncList<int>.Operation.OP_INSERT:
                    // index is where it got added in the list
                    // newItem is the new item
                    break;
                case SyncList<int>.Operation.OP_REMOVEAT:
                    // index is where it got removed in the list
                    // oldItem is the item that was removed
                    break;
                case SyncList<int>.Operation.OP_SET:
                    // index is the index of the item that was updated
                    // oldItem is the previous value for the item at the index
                    // newItem is the new value for the item at the index
                    break;
            }
        }

        [SyncVar(hook = nameof(OnHeroIndexChange))] public int heroIndex = -1;
        void OnHeroIndexChange(int oldVar, int newVar)
        {
            hero = manager.Judgement.heroList.heros[newVar];
            if (isLocalPlayer)
                netCanvas.InitPlayerStatus();

            netCanvas.playerUIs[manager.players.IndexOf(this)].UpdateHero();
        }

        public Hero hero;
        public TBL.Settings.TeamSetting.Team team;


        #region STATUS
        [Header("狀態")]
        [SyncVar] public bool isReady;
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
        #endregion

        TBL.NetCanvas.GameScene netCanvas;
        NetworkRoomManager manager;

        private void Start()
        {
            netCanvas = FindObjectOfType<TBL.NetCanvas.GameScene>();
            manager = ((NetworkRoomManager)NetworkManager.singleton);
            // if (isServer)
            manager.players.Add(this);

            netHandCard.Callback += OnUpdateHandCard;
            netCards.Callback += OnUpdateCard;

            if (isLocalPlayer)
            {
                CmdSetPlayerIndex(manager.GetLocalRoomPlayerIndex());
                CmdSetReady(true);
            }
        }

        public void InitPlayer()
        {
            if (isLocalPlayer)
            {
                CmdDrawTeam();
                CmdDrawHero();
                CmdSetName();
                CmdDrawCard(3);
                // CmdDraw();
            }
            else
            {
                if (heroIndex != -1)
                {
                    hero = manager.Judgement.heroList.heros[heroIndex];
                    netCanvas.playerUIs[manager.players.IndexOf(this)].UpdateHero();
                }
            }
        }

        [Command]
        public void CmdSetName()
        {
            playerName = GameUtils.PlayerName;
        }

        [Command]
        public void CmdDrawCard(int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                netHandCard.Add(manager.DeckManager.DrawCardFromTop().ID);
            }

            // if (manager.Judgement.currentRoundPlayerIndex == playerIndex)
            //     CmdSetDraw(true);
        }

        [Server]
        public void DrawCard(int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                netHandCard.Add(manager.DeckManager.DrawCardFromTop().ID);
            }

            if (manager.Judgement.currentPlayerIndex == playerIndex)
                CmdSetDraw(true);
        }

        // [ClientRpc]
        // public void RpcUpdateCardList()
        // {
        //     if (isLocalPlayer)
        //     {
        //         netCanvas.UpdateCardList();
        //     }

        //     netCanvas.playerUIs[manager.GetPlayerSlotIndex(this)].handCardCount.text = netHandCard.Count.ToString();
        // }

        [Command]
        public void CmdDrawHero()
        {
            int rand;
            do
            {
                rand = UnityEngine.Random.Range(0, manager.Judgement.heroList.heros.Count);
            } while (manager.Judgement.hasUsedHeros.IndexOf(rand) != -1);

            heroIndex = rand;
        }

        [ClientRpc]
        public void RpcUpdateHero(int i)
        {
            heroIndex = i;
            hero = manager.Judgement.heroList.heros[heroIndex];

            if (isLocalPlayer)
                netCanvas.InitPlayerStatus();
        }

        [Command]
        public void CmdDrawTeam()
        {
            team = ((NetworkRoomManager)NetworkManager.singleton).teamList[0];
            ((NetworkRoomManager)NetworkManager.singleton).teamList.RemoveAt(0);

            RpcUpdateTeam((int)team.team);
        }

        [ClientRpc]
        public void RpcUpdateTeam(int i)
        {
            if (!isLocalPlayer)
                return;

            switch (i)
            {
                case (int)TBL.Settings.TeamSetting.TeamEnum.Blue:
                    team = manager.teamSetting.BlueTeam;
                    break;

                case (int)TBL.Settings.TeamSetting.TeamEnum.Red:
                    team = manager.teamSetting.RedTeam;
                    break;

                case (int)TBL.Settings.TeamSetting.TeamEnum.Green:
                    team = manager.teamSetting.GreenTeam;
                    break;
            }

            // FindObjectOfType<TBL.NetCanvas.GameScene>().InitPlayerStatus();
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

        #region UPDATE_UI
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


        #endregion

        #region PLAYER_STATE

        [Command]
        void CmdResetStatus()
        {
            isLocked = false;
            isSkipped = false;
            hasDraw = false;
            acceptCard = false;
            rejectCard = false;
        }

        #endregion

        #region ROUND_ACTION
        [ClientRpc]
        public void RpcUpdateHostPlayer()
        {
            netCanvas.PlayerAnimation(new List<int>() { playerIndex }, "Host");
        }

        [TargetRpc]
        public void TargetDrawStart()
        {
            netCanvas.SetButtonInteractable(draw: 1);
            netCanvas.BindEvent(netCanvas.drawButton.onClick, () => { CmdDrawCard(2); CmdSetDraw(true); });
        }

        [TargetRpc]
        public void TargetRoundStart()
        {

            netCanvas.SetButtonInteractable(draw: 0, send: 1);
            netCanvas.BindEvent(netCanvas.sendButton.onClick, () =>
            {
                netCanvas.CheckCanSend(playerIndex);

                print("Check can send");
            });
            StartCoroutine(RoundUpdate());
        }

        IEnumerator RoundUpdate()
        {
            while (!manager.Judgement.currentRoundHasSendCard)
            {
                yield return null;
            }

            netCanvas.SetButtonInteractable(send: 0);
        }

        [Command]
        public void CmdSendCard(int to, int id)
        {
            netHandCard.Remove(id);

            List<NetworkPlayer> queue = new List<NetworkPlayer>();

            if (CardSetting.IDConvertCard(id).SendType == CardSendType.Direct)
            {
                queue.Add(manager.players[to]);
            }
            else
            {
                int iter = 0;
                int current = to;
                if (Mathf.Abs(to - playerIndex) != 1)
                {
                    if (to > playerIndex)
                        iter = -1;
                    else if (to < playerIndex)
                        iter = 1;
                }
                else
                {
                    iter = to - playerIndex;
                }

                while (queue.Count != manager.players.Count - 1)
                {
                    if (current > manager.players.Count - 1)
                    {
                        current = 0;
                    }
                    else if (current < 0)
                    {
                        current = manager.players.Count - 1;
                    }

                    if (!manager.players[current].isSkipped)
                        queue.Add(manager.players[current]);

                    current += iter;
                }
            }

            queue.Add(this);

            manager.Judgement.cardSendQueue = queue;
            manager.Judgement.currentRoundSendingCardId = id;
            manager.Judgement.currentRoundHasSendCard = true;
        }

        [ClientRpc]
        public void RpcAskCardStart()
        {
            netCanvas.PlayerAnimation(new List<int>() { playerIndex }, "Asking");
            if (isLocalPlayer)
            {
                netCanvas.SetButtonInteractable(accept: 1, reject: 1);
                netCanvas.BindEvent(
                    netCanvas.acceptButton.onClick,
                    () => { CmdSetAcceptCard(true); netCanvas.ClearButtonEvent(); });
                netCanvas.BindEvent(
                    netCanvas.rejectButton.onClick,
                    () =>
                    { CmdSetRejectCard(true); netCanvas.ClearButtonEvent(); });
            }
        }

        [ClientRpc]
        public void RpcAskCardEnd()
        {
            if (isLocalPlayer)
            {
                netCanvas.acceptButton.interactable = false;
                netCanvas.rejectButton.interactable = false;
            }
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

        [TargetRpc]
        public void TargetEndRound()
        {
            StopCoroutine(RoundUpdate());
        }

        [Command]
        public void CmdTestCardAction(CardAction ca)
        {
            manager.DeckManager.Deck.GetCardPrototype(ca.cardID).OnEffect(manager, ca);
            print($"玩家({ca.userIndex}) 對 玩家({ca.targetIndex}) 使用 {CardSetting.IDConvertCard(ca.cardID).GetCardNameFully()}");
        }
        #endregion

        public override void OnStopClient()
        {
            base.OnStopClient();
            OnChatMessage = null;
        }
    }
}

