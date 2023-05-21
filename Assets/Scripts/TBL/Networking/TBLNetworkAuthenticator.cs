using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Jackoo.Utils.CoroutineUtility;

namespace TBL.Networking
{
    public class TBLNetworkAuthenticator : NetworkAuthenticator
    {
        readonly HashSet<NetworkConnection> connectionsPendingDisconnect = new HashSet<NetworkConnection>();
        public static HashSet<string> playerNames = new HashSet<string>();

        #region Message
        public struct AuthRequestMessage : NetworkMessage
        {
            public string authUsername;
            public bool isConnect;
        }

        public struct AuthResponseMessage : NetworkMessage
        {
            public byte code;
            public string message;
        }
        #endregion

        #region Server
        public override void OnStartServer()
        {
            NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
        }

        public override void OnStopServer()
        {
            NetworkClient.UnregisterHandler<AuthRequestMessage>();
            playerNames.Clear();
        }

        public void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthRequestMessage msg)
        {
            print($"Auth request: {msg.authUsername}");

            if (msg.isConnect)
            {
                AuthConnect(conn, msg);
            }
            else
            {
                AuthDisconnect(conn, msg);
            }
        }

        void AuthConnect(NetworkConnectionToClient conn, AuthRequestMessage msg)
        {
            if (connectionsPendingDisconnect.Contains(conn)) return;

            if (!playerNames.Contains(msg.authUsername))
            {
                playerNames.Add(msg.authUsername);

                conn.authenticationData = msg.authUsername;

                AuthResponseMessage authResponseMessage = new AuthResponseMessage
                {
                    code = 100,
                    message = "Success!"
                };

                conn.Send(authResponseMessage);

                ServerAccept(conn);
            }
            else
            {
                connectionsPendingDisconnect.Add(conn);

                AuthResponseMessage authResponseMessage = new AuthResponseMessage
                {
                    code = 200,
                    message = "使用者名稱已存在!"
                };

                conn.Send(authResponseMessage);

                conn.isAuthenticated = false;

                var delay = new Timer(1f, () => connectionsPendingDisconnect.Remove(conn));
            }
        }

        void AuthDisconnect(NetworkConnectionToClient conn, AuthRequestMessage msg)
        {
            playerNames.Remove(msg.authUsername);
        }
        #endregion

        #region Client
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
                authUsername = GameUtils.PlayerName,
                isConnect = true
            };

            NetworkClient.connection.Send(authRequestMessage);
        }

        void RequestRemoveConnection()
        {
            AuthRequestMessage authMsg = new AuthRequestMessage
            {
                authUsername = GameUtils.PlayerName,
                isConnect = false
            };
            NetworkClient.connection.Send(authMsg);
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
        #endregion
    }

}
