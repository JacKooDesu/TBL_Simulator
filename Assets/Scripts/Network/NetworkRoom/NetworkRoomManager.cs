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
        public DeckManager deckManager;
        [SerializeField] GameObject judgementPrefab;
        public NetworkJudgement judgement;

        public List<NetworkPlayer> players = new List<NetworkPlayer>();

        public HeroList heroList;       // 英雄列表
        public TeamSetting teamSetting; // 隊伍圖像、名稱


        public List<TBL.Settings.TeamSetting.Team> teamList = new List<TeamSetting.Team>();

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

        public NetworkPlayer GetLocalPlayer()
        {
            foreach (NetworkPlayer p in players)
            {
                // print(p.netId);
                if (p.isLocalPlayer)
                    return p;
            }

            return null;
        }

        public int GetLocalPlayerSlotIndex()
        {
            int i = 0;
            foreach (NetworkPlayer p in players)
            {
                // print(p.netId);
                if (p.isLocalPlayer)
                    return i;

                ++i;
            }

            return 0;
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

        public int GetLocalRoomPlayerIndex()
        {
            foreach (NetworkRoomPlayer p in roomSlots)
            {
                // print(p.netId);
                if (p.isLocalPlayer)
                    return p.index;
            }

            return 0;
        }

        public void InitTeamList()
        {
            // print("init");
            foreach (TBL.Settings.TeamSetting.TeamPlayerCount setting in teamSetting.teamPlayerCountSetting)
            {
                if (setting.playerCount != roomSlots.Count)
                    continue;

                for (int i = 0; i < setting.Blue; ++i)
                    teamList.Add(teamSetting.BlueTeam);

                for (int i = 0; i < setting.Red; ++i)
                    teamList.Add(teamSetting.RedTeam);

                for (int i = 0; i < setting.Green; ++i)
                    teamList.Add(teamSetting.GreenTeam);
            }

            GameUtils.Shuffle<TBL.Settings.TeamSetting.Team>(ref teamList);
        }
    }
}

