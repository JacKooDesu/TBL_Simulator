using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TBL.Card;
using TBL.Action;
using System;

namespace TBL
{
    public partial class NetworkPlayer : NetworkBehaviour
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

        [Command]
        public void CmdAddHandCard(int id)
        {
            print($"手牌新增 {id}");
            netHandCards.Add((int)id);
        }
        public void AddHandCard(int id)
        {
            print($"手牌新增 {id}");
            netHandCards.Add((int)id);
        }

        public int GetHandCardColorCount(CardColor color)
        {
            int result = 0;
            foreach (var c in netHandCards)
            {
                if (((CardSetting)c).CardColor == color)
                    result++;
            }

            return result;
        }

        public int GetHandCardTypeCount(CardType cardType)
        {
            int result = 0;
            foreach (var c in netHandCards)
            {
                if (((CardSetting)c).CardType == cardType)
                    result++;
            }

            return result;
        }

        public int GetHandCardSendTypeCount(CardSendType sendType)
        {
            int result = 0;
            foreach (var c in netHandCards)
            {
                if (((CardSetting)c).SendType == sendType)
                    result++;
            }

            return result;
        }
        #endregion

        #region NetCard
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
                    netCanvas.heroSkillData.ClearEvent(UnityEngine.EventSystems.EventTriggerType.PointerClick);
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
        public void CmdSetSkillLimited(int index, bool b)
        {
            hero.skills[index].limited = b;
        }

        [Command]
        public void CmdChangeHeroState(bool hiding)
        {
            hero.isHiding = hiding;
            RpcChangeHeroState(hiding);
        }
        [ClientRpc]
        public void RpcChangeHeroState(bool hiding)
        {
            hero.isHiding = hiding;
            RpcUpdateHeroUI();
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
        [SyncVar(hook = nameof(OnTeamIndexChange))] public int teamIndex;
        public void OnTeamIndexChange(int oldVar, int newVar)
        {
            if (!isLocalPlayer)
                return;
        }

        TBL.NetCanvas.GameScene netCanvas;
        NetworkRoomManager manager;

        public override void OnStartClient()
        {
            netCanvas = FindObjectOfType<TBL.NetCanvas.GameScene>();
            manager = ((NetworkRoomManager)NetworkManager.singleton);

            manager.players.Add(this);
        }

        public override void OnStartLocalPlayer()
        {
            netHandCards.Callback += OnUpdateHandCard;
            netCards.Callback += OnUpdateCard;

            StartCoroutine(WaitServerInit());
        }

        IEnumerator WaitServerInit()
        {
            CmdSetPlayerIndex(manager.LocalRoomPlayerIndex);
            CmdSetName();

            yield return new WaitUntil(() => manager.players.Count == manager.roomSlots.Count);
            netCanvas.InitPlayerMapping();
            yield return null;
            CmdSetReady(true);
        }

        [Server]
        public void SetName()
        {
            playerName = GameUtils.PlayerName;
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
                netHandCards.Add(manager.DeckManager.DrawCardFromTop().ID);
            }

            // if (manager.Judgement.currentRoundPlayerIndex == playerIndex)
            //     CmdSetDraw(true);
        }

        [Server]
        public void DrawCard(int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                netHandCards.Add(manager.DeckManager.DrawCardFromTop().ID);
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
        [Server]
        public void DrawHero()
        {
            int rand;
            do
            {
                rand = UnityEngine.Random.Range(0, manager.Judgement.heroList.heros.Count);
            } while (manager.Judgement.hasUsedHeros.IndexOf(rand) != -1);

            manager.Judgement.hasUsedHeros.Add(rand);
            heroIndex = rand;
        }

        [Command]
        public void CmdDrawHero()
        {
            //////////////////////////////////////////////////////////////////
            if (isLocalPlayer)
            {
                heroIndex = 8;
                return;
            }
            //////////////////////////////////////////////////////////////////

            int rand;
            do
            {
                rand = UnityEngine.Random.Range(0, manager.Judgement.heroList.heros.Count);
            } while (manager.Judgement.hasUsedHeros.IndexOf(rand) != -1);

            manager.Judgement.hasUsedHeros.Add(rand);
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

        [Server]
        public void DrawTeam()
        {
            teamIndex = ((NetworkRoomManager)NetworkManager.singleton).teamList[0];
            ((NetworkRoomManager)NetworkManager.singleton).teamList.RemoveAt(0);
        }
        [Command]
        public void CmdDrawTeam()
        {
            teamIndex = ((NetworkRoomManager)NetworkManager.singleton).teamList[0];
            ((NetworkRoomManager)NetworkManager.singleton).teamList.RemoveAt(0);
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
        public void CmdCardHToH(int id, int target)     // Hand To Hand
        {
            print($"玩家 {playerIndex} 給予 玩家 {target} - {Card.CardSetting.IdToCard(id).name}");
            netHandCards.Remove((int)id);
            manager.players[target].AddHandCard(id);
        }

        [Command]
        public void CmdCardHToT(int id, int target)     // Hand to Table
        {
            print($"玩家 {playerIndex} 給予 玩家 {target} - {Card.CardSetting.IdToCard(id).name}");
            netHandCards.Remove((int)id);
            manager.players[target].AddCard(id);
        }

        [Command]
        public void CmdCardHToG(int id) // Hand To Graveyard
        {
            netHandCards.Remove((int)id);
        }

        [Command]
        public void CmdCardTToG(int player, int id) // Table ToGraveyard
        {
            manager.players[player].netCards.Remove(id);
        }

        [Command]
        public void CmdCardTToH(int player, int id)  // Table to Hand
        {
            manager.players[player].netCards.Remove(id);
            manager.players[player].netHandCards.Add(id);
        }

        [Command]
        public void CmdCardHToD(int id)     // Hand to Deck
        {
            manager.DeckManager.CardToTop(id);
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
            netCanvas.tempMenu.InitCustomMenu(
                new List<string>(){
                    msg
                },
                new List<UnityEngine.Events.UnityAction>(){
                    ()=>{
                        if(isDraw)
                            CmdDrawCard(1);
                        CmdAddLog(
                            msg,
                            true,
                            false,
                            new int[]{}
                        );
                    }
                }
            );
        }

        public void UseSkill(int index)
        {
            print($"{hero.skills[index].name} 效果發動");
            hero.skills[index].action.Invoke();
            CmdSetSkillCanActivate(index, false);
        }
        #endregion

        #region LOG
        [Command]
        public void CmdAddLog(string message, bool isServer, bool isPrivate, int[] targetPlayers)
        {
            RpcAddLog(message, isServer, isPrivate, targetPlayers);
        }

        [ClientRpc]
        public void RpcAddLog(string message, bool isServer, bool isPrivate, int[] targetPlayers)
        {
            var log = new UI.LogSystem.LogBase(message, isServer, isPrivate, targetPlayers);
            UI.LogSystem.LogBase.logs.Add(log);

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

            if (targetList.IndexOf(manager.LocalRoomPlayerIndex) != -1)
                netCanvas.AddLog(UI.LogSystem.LogBase.logs.Count - 1);
        }

        [TargetRpc]
        public void TargetAddLog(string message, bool isServer, bool isPrivate, int[] targetPlayers, bool canvasLog)
        {
            var log = new UI.LogSystem.LogBase(message, isServer, isPrivate, targetPlayers);
            UI.LogSystem.LogBase.logs.Add(log);

            if (canvasLog)
                netCanvas.AddLog(UI.LogSystem.LogBase.logs.Count - 1);
        }

        public void AddLog(string message)
        {
            var log = new UI.LogSystem.LogBase(message, false, true, new int[] { playerIndex });
            UI.LogSystem.LogBase.logs.Add(log);

            netCanvas.AddLog(UI.LogSystem.LogBase.logs.Count - 1);
        }
        #endregion

        public override void OnStopClient()
        {
            base.OnStopClient();
            OnChatMessage = null;
        }
    }
}

