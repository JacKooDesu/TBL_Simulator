using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Jackoo.Utils.CoroutineUtility;

namespace TBL.Networking
{
    public partial class NetAuth : NetworkAuthenticator
    {
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
    }

}
