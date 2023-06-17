using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using Mirror;
using Cysharp.Threading.Tasks;

namespace TBL.Game.Networking
{
    public class LocalManager : MonoBehaviour
    {
        public static LocalManager Singleton { get; private set; }
        public bool InitComplete { get; private set; }
        void Awake() => Singleton = this;

        [SerializeField, Range(1, 8)] int playerCount;
        [SerializeField] LocalPlayer playerPrefab;

        [SerializeField] List<LocalPlayer> players = new();
        public List<LocalPlayer> Players => players;
        // LocalPlayer current => Sys.IPlayerStandalone.Me as LocalPlayer;

#if UNITY_EDITOR
        [ContextMenu("Init")]
        void InitPlayer()
        {
            if (playerPrefab == null) return;
            players = new();
            players.AddRange(GetComponentsInChildren<LocalPlayer>());
            for (int i = 0; i < playerCount; ++i)
            {
                var p = PrefabUtility.InstantiatePrefab(playerPrefab, transform) as LocalPlayer;
                players.Add(p);
            }
            var iter = 0;
            foreach (var p in players)
            {
                p.gameObject.name = $"Player {iter}";
                iter++;
            }
        }
#endif

        void Start()
        {
            foreach (var p in Players)
                p.Initialize();
            // SwitchPlayer(0);
            InitComplete = true;
        }

        const KeyCode KEYCODE_BEGIN = KeyCode.Alpha1;
        void Update()
        {
            if (Input.anyKeyDown)
                for (int i = 0; i < playerCount; ++i)
                {
                    if (Input.GetKeyDown(KEYCODE_BEGIN + i))
                    {
                        SwitchPlayer(i);
                        return;
                    }
                }
        }

        void SwitchPlayer(int index)
        {
            var target = players[index];
            Debug.Log($"Switch Player:{Sys.IPlayerStandalone.Me} -> {target}");
            Sys.IPlayerStandalone.Me = target;

            UI.Main.MainUIManager.Singleton.SetupUI(target);
            target.player.ForceUpdateAll();
        }
    }
}
