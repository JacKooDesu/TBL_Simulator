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
        [SerializeField]
        InputField nameField = default;

        protected override void Start()
        {
            nameField.text = GameUtils.PlayerName;

            base.Start();

            host.onClick.AddListener(manager.StartHost);

            client.onClick.AddListener(manager.StartClient);

            hostOnly.onClick.AddListener(manager.StartServer);

            nameField.onEndEdit.AddListener(s => GameUtils.PlayerName = s);

            ip.onValueChanged.AddListener(ip => manager.networkAddress = ip);
        }
    }

}
