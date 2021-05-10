using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace TBL
{
    public class NetworkRoomManager : Mirror.NetworkRoomManager
    {
        public bool showStartButton;
        public override void OnRoomServerPlayersReady()
        {
#if UNITY_SERVER
            base.OnRoomServerPlayersReady();
#else
            showStartButton = true;
#endif
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
    }
}

