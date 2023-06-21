using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace TBL.Game.Networking
{
    using TBL.Game.Sys;
    public class NetworkRoomManager : Mirror.NetworkRoomManager, IStandaloneManager
    {
        public bool showStartButton;
        [SerializeField] GameObject deckManagerPrefab;

        public List<NetworkPlayer> Players { get; } = new();
        public IPlayerStandalone[] GetStandalones() => Players.ToArray();

        public bool InitializeComplete { get; private set; }

        public override void Awake()
        {
            if (singleton != null)
                Destroy(gameObject);
            base.Awake();
            IStandaloneManager.Singleton = this;
        }

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
            if (sceneName == RoomScene)
                InitializeComplete = false;
            if (sceneName == GameplayScene)
                WaitInitStandalone().Forget();
        }

        public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
        {
            var player = Instantiate(playerPrefab);
            Players.Add(player.GetComponent<NetworkPlayer>());
            return player;
        }

        async UniTask WaitInitStandalone()
        {
            await UniTask.WaitUntil(() => roomSlots.Count == Players.Count);
            InitializeComplete = true;
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
        public NetworkPlayer Me() => Players.Find(p => p.isLocalPlayer);

    }
}

