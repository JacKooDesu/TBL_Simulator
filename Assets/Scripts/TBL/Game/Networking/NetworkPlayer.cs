using UnityEngine;
using UnityEngine.Networking;
using Mirror;

namespace TBL.Game.Networking
{
    /// <summary>
    /// 基本只拿來傳訊
    /// </summary>
    public class NetworkPlayer : NetworkBehaviour
    {
        [SyncVar] int index;
        public int Index => index;
    }
}
