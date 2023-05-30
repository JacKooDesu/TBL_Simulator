using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using TBL.Settings;

namespace TBL.Game.Networking
{
    public class NetworkRoomManager : Mirror.NetworkRoomManager
    {
        public static new NetworkRoomManager singleton { get; internal set; }
        public bool showStartButton;
        [SerializeField] GameObject deckManagerPrefab;

        public List<NetworkPlayer> players = new List<NetworkPlayer>();

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

            if (sceneName == GameplayScene)
            {

            }
        }

        public override void OnGUI()
        {
            base.OnGUI();
            if (allPlayersReady && showStartButton && GUI.Button(new Rect(150, 300, 120, 20), "START GAME"))
            {
                showStartButton = false;

                ServerChangeScene(GameplayScene);
            }
        }
        
        /// <summary>
        /// 查找客戶端物件。
        /// </summary>
        /// <returns></returns>
        public NetworkPlayer Me() => players.Find(p => p.isLocalPlayer);
    }
}

