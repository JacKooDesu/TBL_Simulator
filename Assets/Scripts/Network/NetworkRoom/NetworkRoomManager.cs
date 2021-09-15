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

        public List<NetworkPlayer> players = new List<NetworkPlayer>();

        public HeroList heroList;

        public override void OnRoomServerPlayersReady()
        {
#if UNITY_SERVER
            base.OnRoomServerPlayersReady();
#else
            showStartButton = true;
#endif
        }

        public override void OnRoomServerSceneChanged(string sceneName)
        {
            base.OnRoomServerSceneChanged(sceneName);
            if (deckManager == null)
                deckManager = Instantiate(deckManagerPrefab).GetComponent<DeckManager>();

            NetworkServer.Spawn(deckManager.gameObject);
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
                print(p.netId);
                if (p.isLocalPlayer)
                    return p;
            }

            return null;
        }
    }
}

