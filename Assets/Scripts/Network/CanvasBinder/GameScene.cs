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
                Destroy(playerMapping.transform.GetChild(i - 1).gameObject);
            }

            for (int i = 0; i < manager.roomSlots.Count; ++i)
            {
                GameObject g = Instantiate(playerIconPrefab, playerMapping.transform);
                playerUIs.Add(g.GetComponent<TBL.UI.GameScene.PlayerData>());
                playerUIs[i].player = manager.players[i];
            }
        }

        public void PlayerAnimation(List<int> index, string ani)
        {
            for (int i = 0; i < playerUIs.Count; ++i)
            {
                if (index.IndexOf(i) != -1)
                {
                    playerUIs[i].GetComponent<Animator>().SetTrigger(ani);
                }
                else
                {
                    playerUIs[i].GetComponent<Animator>().SetTrigger("Return");
                }
            }
        }

        public void CheckCanSend(int localIndex)
        {
            print($"Player {localIndex} Checking");

            SetButtonInteractable(send: 0);
            // sendButton.gameObject.SetActive(false);

            List<int> sendList = new List<int>();
            if (CardSetting.IDConvertCard(selectCard.cardID).SendType == CardSendType.Direct)
            {
                sendList.AddRange(manager.GetOtherPlayers());
            }
            else
            {
                int last, next;
                last = (localIndex - 1 >= 0 ? localIndex - 1 : manager.players.Count - 1);
                next = (localIndex + 1 > manager.players.Count - 1 ? 0 : localIndex + 1);
                sendList.Add(next);
                sendList.Add(last);
            }

            BindSelectPlayer(
                sendList,
                (i) => { playerUIs[localIndex].player.CmdSendCard(i, selectCard.cardID); });
        }

        public bool isSelectingPlayer = false;
        public void BindSelectPlayer(List<int> list, UnityAction<int> action)
        {
            isSelectingPlayer = true;

            foreach (int i in list)
            {
                UI.GameScene.PlayerData pUI = playerUIs[i];
                JacDev.Utils.EventBinder.Bind(
                    pUI.GetComponent<EventTrigger>(),
                    EventTriggerType.PointerClick,
                    (e) =>
                    {
                        isSelectingPlayer = false;
                        action.Invoke(pUI.player.playerIndex);
                        ResetPlayerUIAnimation();
                        ClearPlayerUIEvent();
                        print($"選擇玩家 {pUI.player.playerIndex}");
                    });

                pUI.GetComponent<Animator>().SetTrigger("Blink");
            }
        }

        public void ResetPlayerUIAnimation()
        {
            foreach (UI.GameScene.PlayerData pUI in playerUIs)
            {
                if (pUI.player.playerIndex == manager.Judgement.currentPlayerIndex)
                    pUI.gameObject.GetComponent<Animator>().SetTrigger("Host");
                else
                    pUI.gameObject.GetComponent<Animator>().SetTrigger("Return");
            }


        }

        public void ClearPlayerUIEvent()
        {
            foreach (UI.GameScene.PlayerData pUI in playerUIs)
            {
                pUI.GetComponent<EventTrigger>().triggers.Clear();
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

            if (player.isServer && s[0] == '/')
                Command.CommandManager.CheckCommand(s.Remove(0, 1));

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
        public TBL.UI.GameScene.CardData selectCard;
        [Header("按鈕")]
        public Button drawButton;
        public Button sendButton;
        public Button acceptButton;
        public Button rejectButton;
        public Button useButton;
        public void SetButtonInteractable(
            int draw = -1,
            int send = -1,
            int accept = -1,
            int reject = -1,
            int use = -1)
        {
            drawButton.interactable = (draw == -1 ? drawButton.interactable : (draw == 1 ? true : false));
            sendButton.interactable = (send == -1 ? sendButton.interactable : (send == 1 ? true : false));
            acceptButton.interactable = (accept == -1 ? acceptButton.interactable : (accept == 1 ? true : false));
            rejectButton.interactable = (reject == -1 ? rejectButton.interactable : (reject == 1 ? true : false));
            useButton.interactable = (use == -1 ? useButton.interactable : (use == 1 ? true : false));
        }

        void BindButtons()
        {
            // drawButton.onClick.AddListener(() =>
            // {
            //     manager.GetLocalPlayer().CmdDrawCard(2);
            //     manager.GetLocalPlayer().CmdSetDraw(true);
            // });

            // useButton.onClick.AddListener(() =>
            // {
            //     BindSelectPlayer(
            //         manager.GetOtherPlayers(),
            //         (x) => manager.GetLocalPlayer().CmdTestCardAction(
            //             new CardAction(
            //                 manager.GetLocalPlayer().playerIndex,
            //                 x,
            //                 selectCard.cardID,
            //                 0
            //             )
            //         ));
            // });

            useButton.onClick.AddListener(() =>
            {
                manager.DeckManager.Deck.GetCardPrototype(selectCard.cardID).OnUse(
                    manager.GetLocalPlayer(), selectCard.cardID);
            });
        }

        public void ClearButtonEvent()
        {
            drawButton.onClick.RemoveAllListeners();
            sendButton.onClick.RemoveAllListeners();
            acceptButton.onClick.RemoveAllListeners();
            rejectButton.onClick.RemoveAllListeners();
            // useButton.onClick.RemoveAllListeners();

            drawButton.interactable = false;
            sendButton.interactable = false;
            acceptButton.interactable = false;
            rejectButton.interactable = false;
            useButton.interactable = false;
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
            heroNameUI.text = manager.GetLocalPlayer().hero.HeroName;

            teamIconUI.sprite = manager.GetLocalPlayer().team.icon;
            teamNameUI.text = manager.GetLocalPlayer().team.name;
        }

        public void UpdateHandCardList()
        {
            for (int i = cardListUI.childCount - 1; i >= 0; --i)
                Destroy(cardListUI.GetChild(i).gameObject);

            foreach (int id in manager.GetLocalPlayer().netHandCard)
            {
                CardSetting tempCard = CardSetting.IDConvertCard(id);
                UI.GameScene.CardData ui = Instantiate(cardTextPrefab, cardListUI).GetComponent<UI.GameScene.CardData>();
                ui.SetUI(tempCard);
            }
        }
        #endregion

        #region TOOLTIP
        [Header("說明欄位")]
        [SerializeField] Text tipTextUI;

        #endregion

        #region TEMP_MENU
        [Header("暫存選單")]
        public UI.GameScene.Menu tempMenu;

        public void ShowPlayerCard(int index, UnityAction<int> action, List<CardColor> requestColor = null)
        {
            if (requestColor == null)
                requestColor = new List<CardColor> { CardColor.Black, CardColor.Red, CardColor.Blue };

            List<int> cardIdList = new List<int>();

            foreach (int i in manager.players[index].netCards)
            {
                if (requestColor.IndexOf(CardSetting.IDConvertCard(i).CardColor) != -1)
                {
                    cardIdList.Add(i);
                }
            }
            tempMenu.InitCardMenu(cardIdList, action);
        }

        public void ShowPlayerHandCard(int index, UnityAction<int> action, List<CardColor> requestColor = null)
        {
            if (requestColor == null)
                requestColor = new List<CardColor> { CardColor.Black, CardColor.Red, CardColor.Blue };

            List<int> cardIdList = new List<int>();

            foreach (int i in manager.players[index].netHandCard)
            {
                if (requestColor.IndexOf(CardSetting.IDConvertCard(i).CardColor) != -1)
                {
                    cardIdList.Add(i);
                }
            }
            tempMenu.InitCardMenu(cardIdList, action);
        }

        public void AskColorCard(UnityAction<int> action, List<CardColor> requestColor)
        {
            tempMenu.InitColorMenu(requestColor, action);
        }
        #endregion

        public Text timeTextUI;

        public void ResetUI()
        {
            tempMenu.Clear();
            ResetPlayerUIAnimation();
            ClearPlayerUIEvent();
        }

        protected override void Start()
        {
            base.Start();

            SetButtonInteractable(0, 0, 0, 0, 0);
            // InitPlayerMapping();
            InitChatWindow();
            BindButtons();

            UILayer = LayerMask.NameToLayer("UI");
        }

        private void Update()
        {
            EventSystem es = EventSystem.current;

            if (selectCard != null && !isSelectingPlayer)
            {
                switch (manager.Judgement.currentPhase)
                {
                    case NetworkJudgement.Phase.ChooseToSend:
                        if (!manager.Judgement.currentRoundHasSendCard && manager.Judgement.currentPlayerIndex == manager.GetLocalPlayer().playerIndex)
                        {
                            SetButtonInteractable(send: 1);
                        }

                        break;

                    case NetworkJudgement.Phase.Draw:
                        SetButtonInteractable(send: 0);
                        break;

                    case NetworkJudgement.Phase.Sending:
                        SetButtonInteractable(send: 0);
                        break;

                    case NetworkJudgement.Phase.Reacting:
                        SetButtonInteractable(send: 0);
                        break;
                }

                if (manager.DeckManager.Deck.GetCardPhaseSetting(selectCard.cardID).FindIndex((x) => x.phase == manager.Judgement.currentPhase) != -1)
                {
                    DeckSetting.CardConfig.PhaseSetting setting = manager.DeckManager.Deck.GetCardPhaseSetting(selectCard.cardID).Find((x) => x.phase == manager.Judgement.currentPhase);

                    if (!setting.roundHost && !setting.sendingHost)
                        SetButtonInteractable(use: 1);

                    if (setting.roundHost)
                    {
                        if (manager.Judgement.currentPlayerIndex != manager.GetLocalPlayer().playerIndex)
                            SetButtonInteractable(use: 0);
                        else
                            SetButtonInteractable(use: 1);
                    }

                    if (setting.sendingHost)
                    {
                        if (manager.Judgement.currentSendingPlayer != manager.GetLocalPlayer().playerIndex)
                            SetButtonInteractable(use: 0);
                        else
                            SetButtonInteractable(use: 1);
                    }
                }
                else
                {
                    SetButtonInteractable(use: 0);
                }
            }
            else
            {
                SetButtonInteractable(send: 0, use: 0);
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (selectCard != null)
                {
                    selectCard.GetComponent<JacDev.Utils.UISlicker.ColorSlicker>().SlickBack();
                    selectCard.isSelected = false;
                    selectCard = null;
                }
            }
        }


        // from: https://forum.unity.com/threads/how-to-detect-if-mouse-is-over-ui.1025533/
        //Returns 'true' if we touched or hovering on Unity UI element.
        int UILayer;

        public GameObject GetPointerHovering()
        {
            return GetPointerOverUIElement(GetEventSystemRaycastResults());
        }


        //Returns 'true' if we touched or hovering on Unity UI element.
        private GameObject GetPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == UILayer)
                    return curRaysastResult.gameObject;
            }
            return null;
        }


        //Gets all event system raycast results of current mouse or touch position.
        static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }
    }
}
