using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TBL.NetCanvas
{
    public class RoomScene : NetCanvasBinderBase
    {
        [SerializeField]
        Button ready = default;
        [SerializeField]
        Button cancel = default;
        [SerializeField]
        Button start = default;

        NetworkRoomPlayer localPlayer;

        protected override void Start()
        {
            base.Start();

            start.interactable = false;
            ready.interactable = true;
            cancel.interactable = false;

            BindEvent(
                ready.onClick,
                () => localPlayer.CmdChangeReadyState(true));

            BindEvent(
                cancel.onClick,
                () => localPlayer.CmdChangeReadyState(false));

            BindEvent(
                start.onClick,
                () => manager.ServerChangeScene(manager.GameplayScene)
            );
        }

        private void Update()
        {
            if (localPlayer == null)
            {
                foreach (NetworkRoomPlayer nrp in manager.roomSlots)
                {
                    if (nrp.isLocalPlayer)
                    {
                        localPlayer = nrp;
                        Debug.Log(localPlayer.name);
                    }
                }
            }
            else
            {
                if (manager.showStartButton)
                    start.interactable = true;

                cancel.interactable = localPlayer.readyToBegin;
                ready.interactable = !localPlayer.readyToBegin;

                if (!localPlayer.isServer)
                    return;

                start.interactable = manager.allPlayersReady;
            }

        }
    }

}
