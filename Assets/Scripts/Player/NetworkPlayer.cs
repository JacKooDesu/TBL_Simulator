using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TBL.Card;
using TBL.Action;
using System;

namespace TBL
{
    public class NetworkPlayer : NetworkBehaviour
    {
        [SyncVar]
        public int playerIndex = 0;

        [Command]
        void CmdSetPlayerIndex(int i)
        {
            playerIndex = i;
        }

        [SyncVar(hook = nameof(OnPlayerNameChange))]
        public string playerName;
        void OnPlayerNameChange(string oldName, string newName)
        {

        }

        [SerializeField, Header("手牌")]
        List<CardObject> handCards = new List<CardObject>();

        [SerializeField, Header("情報")]
        List<CardObject> cards = new List<CardObject>();

        [Header("NetLists")]
        #region  NetHandCard
        public SyncList<int> netHandCard = new SyncList<int>();
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

                    netCanvas.playerUIs[manager.GetPlayerSlotIndex(this)].handCardCount.text = netHandCard.Count.ToString();
                    break;
                case SyncList<int>.Operation.OP_CLEAR:
                    if (isLocalPlayer)
                    {
                        netCanvas.UpdateHandCardList();
                    }

                    netCanvas.playerUIs[manager.GetPlayerSlotIndex(this)].handCardCount.text = netHandCard.Count.ToString();
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

                    netCanvas.playerUIs[manager.GetPlayerSlotIndex(this)].handCardCount.text = netHandCard.Count.ToString();
                    break;
                case SyncList<int>.Operation.OP_SET:
                    // index is the index of the item that was updated
                    // oldItem is the previous value for the item at the index
                    // newItem is the new value for the item at the index
                    break;
            }
        }

        [Command]
        public void CmdAddHandCard(int id)
        {
            print($"手牌新增 {id}");
            netHandCard.Add((int)id);
        }
        public void AddHandCard(int id)
        {
            print($"手牌新增 {id}");
            netHandCard.Add((int)id);
        }
        #endregion

        #region NetCard
        public SyncList<int> netCards = new SyncList<int>();
        // net hand card callback
        public void OnUpdateCard(SyncList<int>.Operation op, int index, int oldItem, int newItem)
        {
            netCanvas.playerUIs[playerIndex].UpdateStatus();

            if (isServer)
                CheckWin();
        }
        public int GetCardColorCount(CardColor color)
        {
            int result = 0;
            foreach (var c in netCards)
            {
                if (((CardSetting)c).CardColor == color)
                    result++;
            }

            return result;
        }

        [Command]
        public void CmdAddCard(int id)
        {
            print($"檯面新增 {id}");
            netCards.Add((int)id);
        }
        public void AddCard(int id)
        {
            print($"檯面新增 {id}");
            netCards.Add((int)id);
        }
        #endregion

        [SyncVar(hook = nameof(OnHeroIndexChange))] public int heroIndex = -1;
        void OnHeroIndexChange(int oldVar, int newVar)
        {
            hero = Instantiate(manager.Judgement.heroList.heros[newVar]);

            hero.Init(this);

            if (isLocalPlayer)
                netCanvas.InitPlayerStatus();

            if (isServer)
            {
                netHeroSkillCanActivate.Clear();
                for (int i = 0; i < hero.skills.Length; ++i)
                    netHeroSkillCanActivate.Add(false);
            }

            netHeroSkillCanActivate.Callback += OnUpdateHeroSkillCanActivate;

            netCanvas.playerUIs[manager.players.IndexOf(this)].UpdateHero();
        }

        public Hero hero;
        public SyncList<bool> netHeroSkillCanActivate = new SyncList<bool>();
        public void OnUpdateHeroSkillCanActivate(SyncList<bool>.Operation op, int index, bool oldItem, bool newItem)
        {
            if (isLocalPlayer)
            {
                print(hero.skills.Length);
                List<string> options = new List<string>();
                List<UnityEngine.Events.UnityAction> actions = new List<UnityEngine.Events.UnityAction>();
                for (int i = 0; i < netHeroSkillCanActivate.Count; ++i)
                {
                    if (netHeroSkillCanActivate[i])
                    {
                        var skill = hero.skills[i];
                        int x = i;
                        if (skill.autoActivate)
                            UseSkill(x);
                        else
                        {
                            options.Add(skill.name);
                            actions.Add(() => UseSkill(x));
                        }
                    }
                }

                if (options.Count == 0)
                {
                    netCanvas.heroSkillData.animator.SetTrigger("Return");
                    return;
                }

                netCanvas.heroSkillData.animator.SetTrigger("Blink");
                netCanvas.heroSkillData.BindEvent(() => netCanvas.tempMenu.InitCustomMenu(options, actions));
            }

        }
        [Command]
        public void CmdSetSkillCanActivate(int index, bool b)
        {
            netHeroSkillCanActivate[index] = b;
        }

        [Command]
        public void CmdChangeHeroState(bool hiding)
        {
            hero.isHiding = hiding;
            RpcChangeHeroState(hiding);
            RpcUpdateHeroUI();
        }
        [ClientRpc]
        public void RpcChangeHeroState(bool hiding)
        {
            hero.isHiding = hiding;
        }

        public TBL.Settings.TeamSetting.Team team;


        #region STATUS
        [Header("狀態")]
        [SyncVar] public bool isReady;
        [Command] public void CmdSetReady(bool b) { isReady = b; }
        // [SyncVar] public bool isReadyLast;

        [SyncVar(hook = nameof(OnLockStatusChange))] public bool isLocked;
        void OnLockStatusChange(bool oldStatus, bool newStatus) { netCanvas.playerUIs[playerIndex].UpdateStatus(); }
        [Command] public void CmdSetLocked(bool b) { isLocked = b; }
        // [SyncVar] public bool isLockedLast;
        [SyncVar(hook = nameof(OnSkipStatusChange))] public bool isSkipped;
        void OnSkipStatusChange(bool oldStatus, bool newStatus) { netCanvas.playerUIs[playerIndex].UpdateStatus(); }
        [Command] public void CmdSetSkipped(bool b) { isSkipped = b; }

        [SyncVar] public bool hasDraw;
        [Command] public void CmdSetDraw(bool b) { hasDraw = b; }

        [SyncVar] public bool acceptCard;
        [Command] public void CmdSetAcceptCard(bool b) { acceptCard = b; }
        [SyncVar] public bool rejectCard;
        [Command] public void CmdSetRejectCard(bool b) { rejectCard = b; }

        public void ResetStatus(int isLocked = -1, int isSkipped = -1, int hasDraw = -1, int acceptCard = -1, int rejectCard = -1)
        {
            if (isLocked != -1)
                this.isLocked = isLocked == 1;

            if (isSkipped != -1)
                this.isSkipped = isSkipped == 1;

            if (hasDraw != -1)
                this.hasDraw = hasDraw == 1;

            if (acceptCard != -1)
                this.acceptCard = acceptCard == 1;

            if (rejectCard != -1)
                this.rejectCard = rejectCard == 1;
        }
        #endregion

        TBL.NetCanvas.GameScene netCanvas;
        NetworkRoomManager manager;

        private void Start()
        {
            netCanvas = FindObjectOfType<TBL.NetCanvas.GameScene>();
            manager = ((NetworkRoomManager)NetworkManager.singleton);
            // if (isServer)
            manager.players.Add(this);

            netHandCard.Callback += OnUpdateHandCard;
            netCards.Callback += OnUpdateCard;

            if (isLocalPlayer)
            {
                CmdSetPlayerIndex(manager.GetLocalRoomPlayerIndex());
                CmdSetReady(true);
            }
        }

        public void InitPlayer()
        {
            if (isLocalPlayer)
            {
                CmdDrawTeam();
                CmdDrawHero();
                CmdSetName();
                CmdDrawCard(3);
                // CmdDraw();
            }
            else
            {
                if (heroIndex != -1)
                {
                    hero = manager.Judgement.heroList.heros[heroIndex];
                    netCanvas.playerUIs[manager.players.IndexOf(this)].UpdateHero();
                }
            }
        }

        [Command]
        public void CmdSetName()
        {
            playerName = GameUtils.PlayerName;
        }

        [Command]
        public void CmdDrawCard(int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                netHandCard.Add(manager.DeckManager.DrawCardFromTop().ID);
            }

            // if (manager.Judgement.currentRoundPlayerIndex == playerIndex)
            //     CmdSetDraw(true);
        }

        [Server]
        public void DrawCard(int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                netHandCard.Add(manager.DeckManager.DrawCardFromTop().ID);
            }

            if (manager.Judgement.currentPlayerIndex == playerIndex)
                CmdSetDraw(true);
        }

        // [ClientRpc]
        // public void RpcUpdateCardList()
        // {
        //     if (isLocalPlayer)
        //     {
        //         netCanvas.UpdateCardList();
        //     }

        //     netCanvas.playerUIs[manager.GetPlayerSlotIndex(this)].handCardCount.text = netHandCard.Count.ToString();
        // }

        [Command]
        public void CmdDrawHero()
        {
            //////////////////////////////////////////////////////////////////
            // if (isLocalPlayer)
            // {
            //     heroIndex = 7;
            //     return;
            // }
            //////////////////////////////////////////////////////////////////

            int rand;
            do
            {
                rand = UnityEngine.Random.Range(0, manager.Judgement.heroList.heros.Count);
            } while (manager.Judgement.hasUsedHeros.IndexOf(rand) != -1);

            heroIndex = rand;
        }

        [ClientRpc]
        public void RpcUpdateHero(int i)
        {
            heroIndex = i;
            hero = manager.Judgement.heroList.heros[heroIndex];

            if (isLocalPlayer)
                netCanvas.InitPlayerStatus();
        }

        public void RpcUpdateHeroUI()
        {
            netCanvas.playerUIs[playerIndex].UpdateHero();
        }

        [Command]
        public void CmdDrawTeam()
        {
            team = ((NetworkRoomManager)NetworkManager.singleton).teamList[0];
            ((NetworkRoomManager)NetworkManager.singleton).teamList.RemoveAt(0);

            RpcUpdateTeam((int)team.team);
        }

        [ClientRpc]
        public void RpcUpdateTeam(int i)
        {
            if (!isLocalPlayer)
                return;

            switch (i)
            {
                case (int)TBL.Settings.TeamSetting.TeamEnum.Blue:
                    team = manager.teamSetting.BlueTeam;
                    break;

                case (int)TBL.Settings.TeamSetting.TeamEnum.Red:
                    team = manager.teamSetting.RedTeam;
                    break;

                case (int)TBL.Settings.TeamSetting.TeamEnum.Green:
                    team = manager.teamSetting.GreenTeam;
                    break;
            }

            // FindObjectOfType<TBL.NetCanvas.GameScene>().InitPlayerStatus();
        }


        #region CHAT
        public static event Action<NetworkPlayer, string> OnChatMessage;
        [Command]
        public void CmdChatMessage(string message)
        {
            if (message.Trim() != "")
            {
                RpcChatReceive(message.Trim());
            }
        }

        [ClientRpc]
        public void RpcChatReceive(string message)
        {
            OnChatMessage?.Invoke(this, message);
        }

        #endregion

        #region UPDATE_UI
        [Command]
        public void CmdUpdatePlayerStatusUI()
        {
            for (int i = 0; i < manager.players.Count; ++i)
                RpcUpdatePlayerStatusUI(i);
        }

        [ClientRpc]
        public void RpcUpdatePlayerStatusUI(int i)
        {
            netCanvas.playerUIs[i].UpdateStatus();
        }


        #endregion

        #region ROUND_ACTION
        [ClientRpc]
        public void RpcUpdateHostPlayer()
        {
            netCanvas.PlayerAnimation(new List<int>() { playerIndex }, "Host");
        }

        [TargetRpc]
        public void TargetDrawStart()
        {
            netCanvas.SetButtonInteractable(draw: 1);
            netCanvas.ClearEvent(netCanvas.drawButton.onClick);
            netCanvas.BindEvent(netCanvas.drawButton.onClick, () => { CmdDrawCard(2); CmdSetDraw(true); });
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
            netHandCard.Remove(id);

            List<NetworkPlayer> queue = new List<NetworkPlayer>();

            if (CardSetting.IDConvertCard(id).SendType == CardSendType.Direct)
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
        public void CmdCardHToH(int id, int target)     // Hand To Hand
        {
            print($"玩家 {playerIndex} 給予 玩家 {target} - {Card.CardSetting.IDConvertCard(id).name}");
            netHandCard.Remove((int)id);
            manager.players[target].AddHandCard(id);
        }

        [Command]
        public void CmdCardHToD(int id, int target)     // Hand to Desk
        {
            print($"玩家 {playerIndex} 給予 玩家 {target} - {Card.CardSetting.IDConvertCard(id).name}");
            netHandCard.Remove((int)id);
            manager.players[target].AddCard(id);
        }

        [Command]
        public void CmdCardHToG(int id) // Hand To Graveyard
        {
            netHandCard.Remove((int)id);
        }

        [Command]
        public void CmdCardTToG(int player, int id) // Table ToGraveyard
        {
            manager.players[player].netCards.Remove(id);
        }

        [Command]
        public void CmdTestCardAction(CardAction ca)
        {
            // .OnEffect(manager, ca);
            netHandCard.Remove(ca.originCardId);
            manager.Judgement.AddCardAction(ca);
            print($"玩家({ca.user}) 對 玩家({ca.target}) 使用 {CardSetting.IDConvertCard(ca.cardId).GetCardNameFully()}");
        }

        public bool CheckWin()
        {
            switch (team.team)
            {
                case Settings.TeamSetting.TeamEnum.Blue:
                    if (GetCardColorCount(CardColor.Blue) >= 3)
                        return true;
                    else
                        return false;

                case Settings.TeamSetting.TeamEnum.Red:
                    if (GetCardColorCount(CardColor.Red) >= 3)
                        return true;
                    else
                        return false;
            }

            return false;
        }

        [TargetRpc]
        public void TargetGetTest()
        {

        }

        public void UseSkill(int index)
        {
            print($"{hero.skills[index].name} 效果發動");
            hero.skills[index].action.Invoke();
        }
        #endregion

        #region LOG
        [ClientRpc]
        public void RpcAddLog(string message, bool isServer, bool isPrivate, int[] targetPlayers)
        {
            var log = new UI.LogBase(message, isServer, isPrivate, targetPlayers);
            UI.LogBase.logs.Add(log);

            List<int> targetList = new List<int>();
            if (log.TargetPlayers.Length == 0)
            {
                for (int i = 0; i < manager.players.Count; ++i)
                {
                    int x = i;
                    targetList.Add(x);
                }
            }
            else
            {
                targetList.AddRange(log.TargetPlayers);
            }

            if (targetList.IndexOf(manager.GetLocalRoomPlayerIndex()) != -1)
                netCanvas.AddLog(UI.LogBase.logs.Count - 1);
        }

        [TargetRpc]
        public void TargetAddLog(string message, bool isServer, bool isPrivate, int[] targetPlayers, bool canvasLog)
        {
            var log = new UI.LogBase(message, isServer, isPrivate, targetPlayers);
            UI.LogBase.logs.Add(log);

            if (canvasLog)
                netCanvas.AddLog(UI.LogBase.logs.Count - 1);
        }

        public void AddLog(string message)
        {
            var log = new UI.LogBase(message, false, true, new int[] { playerIndex });
            UI.LogBase.logs.Add(log);

            netCanvas.AddLog(UI.LogBase.logs.Count - 1);
        }
        #endregion

        public override void OnStopClient()
        {
            base.OnStopClient();
            OnChatMessage = null;
        }
    }
}

