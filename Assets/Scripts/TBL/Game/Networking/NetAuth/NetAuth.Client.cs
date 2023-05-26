using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Jackoo.Utils.CoroutineUtility;

namespace TBL.Networking
{
    public partial class NetAuth : NetworkAuthenticator
    {
        public override void OnStartClient()
        {
            NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
        }

        public override void OnStopClient()
        {
            NetworkClient.UnregisterHandler<AuthResponseMessage>();
        }

        public override void OnClientAuthenticate()
        {
            AuthRequestMessage authRequestMessage = new AuthRequestMessage
            {
                authUsername = GameUtils.PlayerName
            };

            NetworkClient.connection.Send(authRequestMessage);
        }

        public void OnAuthResponseMessage(AuthResponseMessage msg)
        {
            if (msg.code == 100)
            {
                Debug.Log($"Authentication Response: {msg.message}");

                ClientAccept();
            }
            else
            {
                Debug.LogError($"Authentication Response: {msg.message}");

                NetworkManager.singleton.StopHost();

                // // Do this AFTER StopHost so it doesn't get cleared / hidden by OnClientDisconnect
                // LoginUI.instance.errorText.text = msg.message;
                // LoginUI.instance.errorText.gameObject.SetActive(true);
            }
        }
    }

}
