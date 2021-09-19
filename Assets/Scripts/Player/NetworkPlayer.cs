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

                    // netCanvas.playerUIs[manager.GetPlayerSlotIndex(this)].handCardCount.text = netHandCard.Count.ToString();
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

            netCanvas.playerUIs[manager.players.IndexOf(this)].UpdateHero(this);
        }

        public Hero hero;
        public TBL.Settings.TeamSetting.Team team;

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
                CmdSetPlayerIndex(manager.GetLocalRoomPlayerIndex());
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
                    netCanvas.playerUIs[manager.players.IndexOf(this)].UpdateHero(this);
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
                netHandCard.Add(manager.deckManager.DrawCardFromTop().ID);
            }
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

        #region  UPDATE_UI
        [Command]
        public void CmdUpdatePlayerStatusUI()
        {
            for (int i = 0; i < manager.players.Count; ++i)
                RpcUpdatePlayerStatusUI(i);
        }

        [ClientRpc]
        public void RpcUpdatePlayerStatusUI(int i)
        {
            netCanvas.playerUIs[i].UpdateStatus(manager.players[i]);
        }


        #endregion
    }
}

