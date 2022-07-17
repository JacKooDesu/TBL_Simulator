using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Threading.Tasks;

namespace TBL
{
    using Hero;
    using Card;
    using GameAction;

    public partial class NetworkPlayer : NetworkBehaviour
    {
        [SyncVar]
        public int playerIndex;

        [SyncVar(hook = nameof(OnPlayerNameChange))]
        public string playerName;
        void OnPlayerNameChange(string oldName, string newName)
        {

        }

        // use net card instead
        // [SerializeField, Header("手牌")]
        // List<CardObject> handCards = new List<CardObject>();

        // [SerializeField, Header("情報")]
        // List<CardObject> cards = new List<CardObject>();

        [Header("NetLists")]
        public SyncList<int> netHandCards = new SyncList<int>();
        // net hand card callback
        public void OnUpdateHandCard(SyncList<int>.Operation op, int index, int oldItem, int newItem)
        {
            switch (op)
            {
                case SyncList<int>.Operation.OP_ADD:
                    // index is where it got added in the list
                    // newItem is the new item
                    if (isLocalPlayer)
                    {
                        netCanvas.UpdateHandCardList();
                    }

                    netCanvas.playerUIs[manager.GetPlayerSlotIndex(this)].handCardCount.text = netHandCards.Count.ToString();
                    break;
                case SyncList<int>.Operation.OP_CLEAR:
                    if (isLocalPlayer)
                    {
                        netCanvas.UpdateHandCardList();
                    }

                    netCanvas.playerUIs[manager.GetPlayerSlotIndex(this)].handCardCount.text = netHandCards.Count.ToString();
                    // list got cleared
                    break;
                case SyncList<int>.Operation.OP_INSERT:
                    // index is where it got added in the list
                    // newItem is the new item
                    break;
                case SyncList<int>.Operation.OP_REMOVEAT:
                    // index is where it got removed in the list
                    // oldItem is the item that was removed
                    if (isLocalPlayer)
                    {
                        netCanvas.UpdateHandCardList();
                    }

                    netCanvas.playerUIs[manager.GetPlayerSlotIndex(this)].handCardCount.text = netHandCards.Count.ToString();
                    break;
                case SyncList<int>.Operation.OP_SET:
                    // index is the index of the item that was updated
                    // oldItem is the previous value for the item at the index
                    // newItem is the new value for the item at the index
                    break;
            }
        }

        public SyncList<int> netCards = new SyncList<int>();
        // net hand card callback
        public void OnUpdateCard(SyncList<int>.Operation op, int index, int oldItem, int newItem)
        {
            netCanvas.playerUIs[playerIndex].UpdateStatus();

            if (isServer)
            {
                if (GetCardColorCount(CardColor.Black) >= 3)
                    isDead = true;

                CheckWin();
            }
        }

        public TBL.Settings.TeamSetting.Team Team
        {
            get
            {
                switch (teamIndex)
                {
                    case (int)TBL.Settings.TeamSetting.TeamEnum.Blue:
                        return manager.teamSetting.BlueTeam;

                    case (int)TBL.Settings.TeamSetting.TeamEnum.Red:
                        return manager.teamSetting.RedTeam;

                    case (int)TBL.Settings.TeamSetting.TeamEnum.Green:
                        return manager.teamSetting.GreenTeam;
                }

                Debug.LogWarning("Net Player : Unknow team index");
                return null;
            }
        }
        [SyncVar(hook = nameof(OnTeamIndexChange)), HideInInspector] public int teamIndex;
        public void OnTeamIndexChange(int oldVar, int newVar)
        {
            if (!isLocalPlayer)
                return;
        }

        TBL.NetCanvas.GameScene netCanvas;
        NetworkRoomManager manager;
        NetworkJudgement judgement;

        public override void OnStartClient()
        {
            netCanvas = FindObjectOfType<TBL.NetCanvas.GameScene>();
            manager = ((NetworkRoomManager)NetworkManager.singleton);
            judgement = manager.Judgement;

            playerIndex = Int32.MaxValue;

            manager.players.Add(this);
        }

        public override void OnStartLocalPlayer()
        {
            netHandCards.Callback += OnUpdateHandCard;
            netCards.Callback += OnUpdateCard;

            StartCoroutine(WaitServerInit());
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            OnChatMessage = null;
        }

        IEnumerator WaitServerInit()
        {
            CmdSetPlayerIndex(manager.LocalRoomPlayerIndex);
            CmdSetName();

            yield return new WaitUntil(() => manager.players.Count == manager.roomSlots.Count);
            yield return new WaitUntil(() => manager.players.Find(p => p.playerIndex == Int32.MaxValue) == null);

            manager.SortPlayers();
            netCanvas.InitPlayerMapping();

            yield return null;
            CmdSetReady(true);
        }

        [Command]
        void CmdSetPlayerIndex(int i)
        {
            playerIndex = i;
        }

        [ClientRpc]
        public void RpcUpdateHeroUI()
        {
            netCanvas.playerUIs[playerIndex].UpdateHero();
        }

        #region ROUND_ACTION
        [ClientRpc]
        public void RpcUpdateRoundHost()
        {
            netCanvas.PlayerAnimation(new List<int>() { playerIndex }, "Host");
        }

        [TargetRpc]
        public void TargetDrawStart()
        {
            netCanvas.SetButtonInteractable(draw: 1);
            netCanvas.ClearEvent(netCanvas.drawButton.onClick);
            netCanvas.BindEvent(netCanvas.drawButton.onClick, () => CmdSetDraw(true));
        }

        [TargetRpc]
        public void TargetEndRound()
        {
            netCanvas.isSelectingPlayer = false;
            // StopCoroutine(RoundUpdate());
        }

        [TargetRpc]
        public void TargetRoundStart()
        {

            netCanvas.SetButtonInteractable(draw: 0, send: 1);
            netCanvas.BindEvent(netCanvas.sendButton.onClick, () =>
            {
                netCanvas.CheckCanSend(playerIndex);

                print("Check can send");
            });
            StartCoroutine(RoundUpdate());
        }

        IEnumerator RoundUpdate()
        {
            while (!manager.Judgement.currentRoundHasSendCard)
            {
                yield return null;
            }

            netCanvas.SetButtonInteractable(send: 0);
        }

        [Command]
        public void CmdSendCard(int to, int id)
        {
            netHandCards.Remove(id);

            List<NetworkPlayer> queue = new List<NetworkPlayer>();

            if (CardSetting.IdToCard(id).SendType == CardSendType.Direct)
            {
                queue.Add(manager.players[to]);
            }
            else
            {
                int iter = 0;
                int current = to;
                if (Mathf.Abs(to - playerIndex) != 1)
                {
                    if (to > playerIndex)
                        iter = -1;
                    else if (to < playerIndex)
                        iter = 1;
                }
                else
                {
                    iter = to - playerIndex;
                }

                while (queue.Count != manager.players.Count - 1)
                {
                    if (current > manager.players.Count - 1)
                    {
                        current = 0;
                    }
                    else if (current < 0)
                    {
                        current = manager.players.Count - 1;
                    }

                    if (!manager.players[current].isSkipped)
                        queue.Add(manager.players[current]);

                    current += iter;
                }
            }

            queue.Add(this);

            manager.Judgement.cardSendQueue = queue;
            manager.Judgement.currentRoundSendingCardId = id;
            manager.Judgement.currentRoundHasSendCard = true;
        }

        [ClientRpc]
        public void RpcAskCardStart(int id)
        {
            netCanvas.PlayerAnimation(new List<int>() { playerIndex }, "Asking");
            if (isLocalPlayer)
            {
                var card = (CardSetting)(id);
                string message = "情報來了！\n";
                message += "這是一份 ";
                if (card.SendType == CardSendType.Public)
                    message += $"公開文本 - {RichTextHelper.TextWithColor(card.CardName, card.Color)}";
                else
                    message += (card.SendType == Card.CardSendType.Secret ? "密電" : "直達密電");
                UI.GameScene.TipCanvas.Singleton.Show(message, true);

                netCanvas.SetButtonInteractable(accept: 1, reject: 1);
                netCanvas.BindEvent(
                    netCanvas.acceptButton.onClick,
                    () => { CmdSetAcceptCard(true); netCanvas.ClearButtonEvent(); });
                netCanvas.BindEvent(
                    netCanvas.rejectButton.onClick,
                    () =>
                    { CmdSetRejectCard(true); netCanvas.ClearButtonEvent(); });
            }
        }

        [ClientRpc]
        public void RpcAskCardEnd()
        {
            if (isLocalPlayer)
            {
                netCanvas.acceptButton.interactable = false;
                netCanvas.rejectButton.interactable = false;

                UI.GameScene.TipCanvas.Singleton.ResetPriorityMessage();
            }
        }

        [Command]
        public void CmdTestCardAction(CardAction ca)
        {
            // .OnEffect(manager, ca);
            netHandCards.Remove(ca.originCardId);
            manager.Judgement.AddCardAction(ca);
            print($"玩家({ca.user}) 對 玩家({ca.target}) 使用 {CardSetting.IdToCard(ca.cardId).GetCardNameFully()}");
        }

        public void CheckWin()
        {
            if (isDead)
                return;

            switch (Team.team)
            {
                case Settings.TeamSetting.TeamEnum.Blue:
                    if (GetCardColorCount(CardColor.Blue) >= 3 || manager.GetTeamPlayerCount(Settings.TeamSetting.TeamEnum.Red) == 0)
                        this.isWin = true;
                    break;

                case Settings.TeamSetting.TeamEnum.Red:
                    if (GetCardColorCount(CardColor.Red) >= 3 || manager.GetTeamPlayerCount(Settings.TeamSetting.TeamEnum.Blue) == 0)
                        this.isWin = true;
                    break;

                case Settings.TeamSetting.TeamEnum.Green:
                    if (hero.mission != null)
                        this.isWin = hero.mission.checker.Invoke();
                    break;
            }
        }

        [TargetRpc]
        public void TargetGetTest(bool isDraw)
        {
            string msg = $"玩家 {playerIndex} ({playerName}) ：" + (isDraw ? "抽一張牌" : "我是一個好人");
            var option = new Option
            {
                str = msg,
                onSelect = () =>
                {
                    if (isDraw)
                        CmdDrawCard(1);
                    CmdAddLog(
                        msg,
                        true,
                        false,
                        new int[] { }
                    );
                },
                type = OptionType.OTHER
            };
            netCanvas.InitMenu(new List<Option> { option });
        }

        [Command]
        public void CmdUseSkill(SkillAction skillAction)
        {
            judgement.UseSkill(skillAction);
        }
        #endregion
    }
}

