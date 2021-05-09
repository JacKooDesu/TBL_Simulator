using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TBL.NetCanvas
{
    public class OfflineScene : NetCanvasBinderBase
    {
        [SerializeField]
        Button host = default;
        [SerializeField]
        Button client = default;
        [SerializeField]
        InputField ip = default;
        [SerializeField]
        Button hostOnly = default;

        protected override void Start()
        {
            base.Start();

            BindEvent(
                host.onClick,
                () => manager.StartHost()
            );

            BindEvent(
                client.onClick, () =>
                {
                    manager.networkAddress = ip.text;
                    manager.StartClient();
                });

            BindEvent(
                hostOnly.onClick,
                () => manager.StartServer()
            );
        }
    }

}
