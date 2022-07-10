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
    using Card;

    public partial class GameScene : NetCanvasBinderBase
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
            if (CardSetting.IdToCard(selectCard.cardID).SendType == CardSendType.Direct)
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
                    manager.LocalPlayer, selectCard.cardID);
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
        public TBL.UI.GameScene.HeroSkillData heroSkillData;
        [SerializeField] Image heroAvatarUI;
        [SerializeField] Text heroNameUI;
        [SerializeField] Image teamIconUI;
        [SerializeField] Text teamNameUI;
        [SerializeField] Transform cardListUI;
        [SerializeField] GameObject cardTextPrefab;

        public void InitPlayerStatus()
        {
            heroAvatarUI.sprite = manager.LocalPlayer.hero.Avatar;
            heroNameUI.text = manager.LocalPlayer.hero.HeroName;

            teamIconUI.sprite = manager.LocalPlayer.Team.icon;
            teamNameUI.text = manager.LocalPlayer.Team.name;

            heroSkillData.Init(manager.LocalPlayer.hero);
        }

        public void UpdateHandCardList()
        {
            for (int i = cardListUI.childCount - 1; i >= 0; --i)
                Destroy(cardListUI.GetChild(i).gameObject);

            foreach (int id in manager.LocalPlayer.netHandCards)
            {
                CardSetting tempCard = CardSetting.IdToCard(id);
                UI.GameScene.CardData ui = Instantiate(cardTextPrefab, cardListUI).GetComponent<UI.GameScene.CardData>();
                ui.SetUI(tempCard);
            }
        }
        #endregion

        #region TOOLTIP
        [Header("說明欄位")]
        [SerializeField] Text tipTextUI;

        #endregion

        public Text timeTextUI;

        public void ResetUI()
        {
            tempMenu.Clear();
            ResetPlayerUIAnimation();
            ClearPlayerUIEvent();
        }

        #region LOG
        public ScrollRect logScrollRect;
        public Text logText;

        public void AddLog(int index)
        {
            var log = UI.LogSystem.LogBase.logs[index];
            string prefix = $"[{(log.IsPrivate ? "私人" : "公開")}]";
            prefix = RichTextHelper.TextWithStyles(
                prefix,
                log.IsPrivate ?
                    new RichTextHelper.Setting<Color>(RichTextHelper.Style.Color, new Color(.6f, 0f, 0f)) :
                    new RichTextHelper.Setting<Color>(RichTextHelper.Style.Color, new Color(0f, .6f, 0f)),
                new RichTextHelper.SettingBase(RichTextHelper.Style.Bold));

            logText.text += $"{prefix} {UI.LogSystem.LogBase.logs[index].Message}\n";

            logScrollRect.verticalNormalizedPosition = -1;
        }

        #endregion

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
                        if (!manager.Judgement.currentRoundHasSendCard && manager.Judgement.currentPlayerIndex == manager.LocalPlayer.playerIndex)
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
                        if (manager.Judgement.currentPlayerIndex != manager.LocalPlayer.playerIndex)
                            SetButtonInteractable(use: 0);
                        else
                            SetButtonInteractable(use: 1);
                    }

                    if (setting.sendingHost)
                    {
                        if (manager.Judgement.currentSendingPlayer != manager.LocalPlayer.playerIndex)
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
