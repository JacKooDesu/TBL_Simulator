using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Jackoo.Utils.CoroutineUtility;

namespace TBL.Networking
{
    public partial class NetAuth : NetworkAuthenticator
    {
        readonly HashSet<NetworkConnection> connectionsPendingDisconnect = new HashSet<NetworkConnection>();
        public static HashSet<string> playerNames = new HashSet<string>();

        #region Message
        public struct AuthRequestMessage : NetworkMessage
        {
            public string authUsername;
        }

        public struct AuthResponseMessage : NetworkMessage
        {
            public byte code;
            public string message;
        }
        #endregion
    }

}
