using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBL.NetCanvas;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Mirror;
using TBL.Card;


namespace TBL.NetCanvas
{
    public class GameScene : NetCanvasBinderBase
    {
        #region PLAYER_MAPPING
        [SerializeField]
        GameObject playerMapping;
        [SerializeField]
        GameObject playerIconPrefab;

        public List<TBL.UI.GameScene.PlayerData> playerUIs = new List<UI.GameScene.PlayerData>();
        public void InitPlayerMapping()
        {
            for (int i = playerMapping.transform.childCount; i >= 1; --i)
            {
                Destroy(playerMapping.transform.GetChild(i-1).gameObject);
            }

            for (int i = 0; i < manager.roomSlots.Count; ++i)
            {
                GameObject g = Instantiate(playerIconPrefab, playerMapping.transform);
                playerUIs.Add(g.GetComponent<TBL.UI.GameScene.PlayerData>());
            }
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
        [SerializeField] Transform cardListUI;
        [SerializeField] GameObject cardTextPrefab;

        public void InitPlayerStatus()
        {
            heroAvatarUI.sprite = manager.GetLocalPlayer().hero.Avatar;
            heroNameUI.text = manager.GetLocalPlayer().hero.name;

            teamIconUI.sprite = manager.GetLocalPlayer().team.icon;
            teamNameUI.text = manager.GetLocalPlayer().team.name;
        }

        public void UpdateCardList()
        {
            for (int i = cardListUI.childCount - 1; i >= 0; --i)
                Destroy(cardListUI.GetChild(i).gameObject);

            foreach (int id in manager.GetLocalPlayer().netHandCard)
            {
                CardSetting tempCard = CardSetting.IDConvertCard((ushort)id);
                Text cardName = Instantiate(cardTextPrefab, cardListUI).GetComponent<Text>();
                cardName.text = tempCard.CardName;
                cardName.color = tempCard.Color;
            }
        }
        #endregion

        #region TOOLTIP
        [Header("說明欄位")]
        [SerializeField] Text tipTextUI;

        #endregion

        protected override void Start()
        {
            base.Start();
            // InitPlayerMapping();
            InitChatWindow();
            BindButtons();
        }
    }
}
