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
        public string playerName;

        [SerializeField, Header("手牌")]
        List<CardObject> handCards = new List<CardObject>();

        [SerializeField, Header("情報")]
        List<CardObject> cards = new List<CardObject>();

        [Header("NetLists")]
        public SyncList<int> netHandCard = new SyncList<int>();
        public SyncList<int> netCards = new SyncList<int>();

        [SyncVar] public int heroIndex = -1;
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

            if (isLocalPlayer)
            {
                CmdDrawTeam();
                CmdDrawHero();
                CmdSetName();
                CmdDraw();
            }
            else if (!isServer)
            {
                if (heroIndex != -1)
                {
                    hero = manager.heroList.heros[heroIndex];
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
        public void CmdDraw()
        {
            netHandCard.Add(manager.deckManager.DrawCardFromTop().ID);
        }

        [Command]
        public void CmdDrawHero()
        {
            int rand = UnityEngine.Random.Range(0, manager.heroList.heros.Count);
            hero = manager.heroList.heros[rand];
            heroIndex = rand;
            RpcUpdateHero(rand);

            RpcUpdatePlayerHeroUI();
        }

        [ClientRpc]
        public void RpcUpdateHero(int i)
        {
            heroIndex = i;
            hero = manager.heroList.heros[heroIndex];

            if(isLocalPlayer)
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
        public void CmdUpdatePlayerHeroUI()
        {

        }

        [ClientRpc]
        public void RpcUpdatePlayerHeroUI()
        {
            netCanvas.playerUIs[manager.players.IndexOf(this)].UpdateHero(this);
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
            netCanvas.playerUIs[i].UpdateStatus(manager.players[i]);
        }
        #endregion
    }
}

