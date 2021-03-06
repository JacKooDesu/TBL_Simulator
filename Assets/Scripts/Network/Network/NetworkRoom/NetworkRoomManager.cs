using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using TBL.Settings;

namespace TBL
{
    public class NetworkRoomManager : Mirror.NetworkRoomManager
    {
        public bool showStartButton;
        [SerializeField] GameObject deckManagerPrefab;
        DeckManager deckManager;
        public DeckManager DeckManager
        {
            get
            {
                if (deckManager == null)
                    deckManager = FindObjectOfType<DeckManager>();

                return deckManager;
            }
        }
        [SerializeField] GameObject judgementPrefab;
        NetworkJudgement judgement;
        public NetworkJudgement Judgement
        {
            get
            {
                if (judgement == null)
                    judgement = FindObjectOfType<NetworkJudgement>();

                return judgement;
            }
        }

        public List<NetworkPlayer> players = new List<NetworkPlayer>();

        public TeamSetting teamSetting; // 隊伍圖像、名稱


        public List<int> teamList = new List<int>();

        public override void OnRoomServerPlayersReady()
        {
#if UNITY_SERVER
            base.OnRoomServerPlayersReady();
#else
            showStartButton = true;
            InitTeamList();
#endif
        }

        public override void OnRoomServerSceneChanged(string sceneName)
        {
            base.OnRoomServerSceneChanged(sceneName);

            if (sceneName == GameplayScene)
            {
                if (deckManager == null)
                    deckManager = Instantiate(deckManagerPrefab).GetComponent<DeckManager>();

                NetworkServer.Spawn(deckManager.gameObject);


                if (judgement == null)
                    judgement = Instantiate(judgementPrefab).GetComponent<NetworkJudgement>();

                NetworkServer.Spawn(judgement.gameObject);
            }
        }

        public override void OnGUI()
        {
            base.OnGUI();

            // if (allPlayersReady && showStartButton && GUI.Button(new Rect(150, 300, 120, 20), "START GAME"))
            // {
            //     // set to false to hide it in the game scene
            //     showStartButton = false;

            //     ServerChangeScene(GameplayScene);
            // }
        }

        #region  Net Player
        public void SortPlayers()
        {
            var tempPlayers = new NetworkPlayer[players.Count];
            foreach (var p in players)
            {
                tempPlayers[p.playerIndex] = p;
            }

            players = new List<NetworkPlayer>(tempPlayers);
        }

        public NetworkPlayer LocalPlayer
        {
            get => players.Find(p => p.isLocalPlayer);
            // foreach (NetworkPlayer p in players)
            // {
            //     // print(p.netId);
            //     if (p.isLocalPlayer)
            //         return p;
            // }

            // return null;
        }

        public int LocalPlayerIndex
        {
            get => players.FindIndex(p => p.isLocalPlayer);
            // int i = 0;
            // foreach (NetworkPlayer p in players)
            // {
            //     // print(p.netId);
            //     if (p.isLocalPlayer)
            //         return i;

            //     ++i;
            // }

            // return 0;
        }
        public int GetPlayerSlotIndex(NetworkPlayer player)
        {
            int i = 0;
            foreach (NetworkPlayer p in players)
            {
                if (p == player)
                    return i;

                ++i;
            }

            return 0;
        }

        public List<int> GetOtherPlayers()
        {
            List<int> list = new List<int>();
            foreach (NetworkPlayer p in players)
            {
                if (!p.isLocalPlayer)
                    list.Add(p.playerIndex);
            }
            return list;
        }

        public List<int> GetAllPlayers()
        {
            List<int> list = new List<int>();
            foreach (NetworkPlayer p in players)
            {
                list.Add(p.playerIndex);
            }
            return list;
        }
        #endregion
        #region Net Room Player Getter
        public NetworkRoomPlayer LocalRoomPlayer
        {
            get => roomSlots.Find(p => p.isLocalPlayer) as TBL.NetworkRoomPlayer;
        }
        public int LocalRoomPlayerIndex
        {
            get => roomSlots.FindIndex(p => p.hasAuthority);
        }
        #endregion

        public void InitTeamList()
        {
            // print("init");
            foreach (TBL.Settings.TeamSetting.TeamPlayerCount setting in teamSetting.teamPlayerCountSetting)
            {
                if (setting.playerCount != roomSlots.Count)
                    continue;

                for (int i = 0; i < setting.Blue; ++i)
                    teamList.Add(((int)TeamSetting.TeamEnum.Blue));

                for (int i = 0; i < setting.Red; ++i)
                    teamList.Add(((int)TeamSetting.TeamEnum.Red));

                for (int i = 0; i < setting.Green; ++i)
                    teamList.Add(((int)TeamSetting.TeamEnum.Green));
            }

            GameUtils.Shuffle<int>(ref teamList);
        }

        public void CheckAllHeroSkill()
        {
            foreach (var p in players)
                p.hero.CheckSkill();
        }

        public void TargetLog(UI.LogSystem.LogBase log)
        {
            List<int> targetList = new List<int>(); ;
            if (log.TargetPlayers.Length == 0)
            {
                for (int i = 0; i < players.Count; ++i)
                {
                    int x = i;
                    targetList.Add(x);
                }
            }
            else
            {
                targetList.AddRange(log.TargetPlayers);
            }

            foreach (var p in players)
            {
                p.TargetAddLog(log.Message, log.IsServer, log.IsPrivate, log.TargetPlayers, targetList.IndexOf(p.playerIndex) != -1);
            }
        }

        public void TargetLogAll(UI.LogSystem.LogBase log)
        {
            foreach (var p in players)
            {
                p.TargetAddLog(log.Message, log.IsServer, log.IsPrivate, log.TargetPlayers, true);
            }
        }

        public void RpcLog(UI.LogSystem.LogBase log, NetworkPlayer player)
        {
            player.RpcAddLog(log.Message, log.IsServer, log.IsPrivate, log.TargetPlayers);
        }

        public void CheckAllWin()
        {
            foreach (var p in players)
            {
                p.CheckWin();
            }
        }

        public int GetDeadPlayerCount()
        {
            int iter = 0;
            foreach (var p in players)
            {
                if (p.isDead)
                    iter++;
            }
            return iter;
        }

        public int GetTeamPlayerCount(TeamSetting.TeamEnum team)
        {
            int iter = 0;
            foreach (var p in players)
            {
                if (p.Team.team == team)
                    iter++;
            }
            return iter;
        }
    }
}

