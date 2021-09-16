using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBL.NetCanvas;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Mirror;


namespace TBL.NetCanvas
{
    public class GameScene : NetCanvasBinderBase
    {
        #region PLAYER_MAPPING
        [SerializeField]
        GameObject playerMapping;
        [SerializeField]
        GameObject playerIconPrefab;
        [SerializeField]
        int testPlayerCount = 5;

        public List<TBL.UI.GameScene.PlayerData> playerUIs = new List<UI.GameScene.PlayerData>();
        void InitPlayerMapping()
        {
            for (int i = 0; i < manager.roomSlots.Count; ++i)
            {
                GameObject g = Instantiate(playerIconPrefab, playerMapping.transform);
                playerUIs.Add(g.GetComponent<TBL.UI.GameScene.PlayerData>());
            }
            // ((RectTransform)playerMapping.transform).sizeDelta = new Vector2(
            //     ((RectTransform)playerMapping.transform).sizeDelta.x,
            //     ((RectTransform)playerIconPrefab.transform).sizeDelta.y * testPlayerCount + playerMapping.GetComponent<VerticalLayoutGroup>().spacing * (testPlayerCount - 1));
        }
        #endregion

        #region CHAT
        [Header("聊天室")]
        [SerializeField]
        InputField chatInput;
        InputFieldSubmit chatSubmit;
        [SerializeField]
        ScrollRect chatScroll;
        [SerializeField]
        Text chatHistory;
        void InitChatWindow()
        {
            chatSubmit = chatInput.GetComponent<InputFieldSubmit>();
            NetworkPlayer.OnChatMessage += OnPlayerChatMessage;
            chatSubmit.onSubmit.AddListener(OnChatMessageSubmit);
        }

        // Submit event
        public void OnChatMessageSubmit(string s)
        {
            if (chatInput.text.Trim() == "")
                return;

            print("send");

            NetworkPlayer player = NetworkClient.connection.identity.GetComponent<NetworkPlayer>();

            player.CmdChatMessage(chatInput.text.Trim());

            chatInput.text = "";
        }

        // On message evnet
        void OnPlayerChatMessage(NetworkPlayer player, string message)
        {
            string prettyMessage = player.isLocalPlayer ?
                $"<color=red>{player.playerName}: </color> {message}" :
                $"<color=blue>{player.playerName}: </color> {message}";

            StartCoroutine(AppendAndScroll(prettyMessage));
        }

        // Add message
        IEnumerator AppendAndScroll(string s)
        {
            chatHistory.text += s + "\n";

            // it takes 2 frames for the UI to update ?!?!
            yield return null;
            yield return null;

            // slam the scrollbar down
            chatScroll.verticalScrollbar.value = 0;
        }
        #endregion

        #region BUTTON
        [Header("按鈕")]
        [SerializeField]
        Button drawButton;
        [SerializeField]
        Button useButton;

        void BindButtons()
        {
            drawButton.onClick.AddListener(() => { manager.GetLocalPlayer().CmdDraw(); });
        }

        #endregion

        #region PLAYER_STATUS
        [Header("玩家資訊")]
        [SerializeField] Image heroAvatarUI;
        [SerializeField] Text heroNameUI;
        [SerializeField] Image teamIconUI;
        [SerializeField] Text teamNameUI;

        public void InitPlayerStatus()
        {
            heroAvatarUI.sprite = manager.GetLocalPlayer().hero.Avatar;
            heroNameUI.text = manager.GetLocalPlayer().hero.name;

            teamIconUI.sprite = manager.GetLocalPlayer().team.icon;
            teamNameUI.text = manager.GetLocalPlayer().team.name;
        }
        #endregion

        protected override void Start()
        {
            base.Start();
            InitPlayerMapping();
            InitChatWindow();
            BindButtons();
        }
    }
}

