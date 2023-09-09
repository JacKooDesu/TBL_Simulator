using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
namespace TBL.Game.Sys
{
    using Utils;
    using Setting;
    using Networking;
    using NetworkManager = Networking.NetworkRoomManager;
    using LocalManager = Networking.LocalManager;
    using QuestType = PhaseQuestStatus.QuestType;
    using System.Collections.Generic;

    /// <summary>
    /// 資源管理，伺服端使用操作所有遊戲物件。
    /// </summary>
    public class Manager : MonoBehaviour
    {
        [SerializeField] DeckSetting deckSetting;
        [SerializeField] Deck deck = new Deck();
        public Deck Deck => deck;

        [SerializeField] TeamSetting teamSetting;
        [SerializeField] HeroSetting heroSetting;
        [SerializeField] PlayerList players = new PlayerList();
        public PlayerList Players => players;

        int currentPlayerIndex = -1;
        public Player CurrentPlayer => Players[currentPlayerIndex];

        PhaseManager phaseManager;
        public PhaseManager PhaseManager => phaseManager;

        [SerializeField] HeroManager heroManager = new();
        public HeroManager HeroManager => heroManager;

        public delegate bool GetManagerDelegate(out Manager manager);
        public static GetManagerDelegate TryGet() => (out Manager manager) => manager = Instance;
        public static Manager Instance { get; private set; }

        async UniTask Start()
        {
            if (!InitManager())
                return;

            await InitGame();

            players.SetupTeam(teamSetting, heroSetting, true);

            foreach (var p in players.List)
            {
                heroManager.SetupForPlayer(p, this);
                Draw(p, 7);
            }

            Broadcast(new GameStartPacket(), SendType.Target);

            NewRound();
            var ct = gameObject.GetCancellationTokenOnDestroy();
            phaseManager.Run(ct).Forget();
        }

        bool InitManager()
        {
            Instance = this;

            if (IStandaloneManager.Singleton == null)
                return false;

            deck.Init(deckSetting)
                .sleeping.AddRange(deck.CardDatas)
                .Shuffle();
            phaseManager = new(this);
            return true;
        }

        async UniTask InitGame()
        {
            await UniTask.WaitUntil(() => IStandaloneManager.Singleton.InitializeComplete);

            IPlayerStandalone[] standalones = null!;
            standalones = IStandaloneManager.Singleton.GetStandalones();

            players.Init(standalones);
            Broadcast(new ServerReadyPacket(), SendType.Target);

            await UniTask.WaitUntil(() => standalones.FirstOrDefault(x => x.IsReady == false) == null);
        }

        public void NewRound()
        {
            currentPlayerIndex++;
            if (currentPlayerIndex >= players.List.Count)
                currentPlayerIndex = 0;

            phaseManager.ResetFlow();

            Broadcast(
                new NewRoundPacket { HostId = CurrentPlayer.ProfileStatus.Id },
                SendType.Target
            );
        }

        public void Draw(Player p, int count) =>
            p.CardStatus.AddHandCards(deck.Draw(count).ToIdList().ToArray());
        public void Draw(int id, int count) => Draw(Players.QueryById(id), count);
        public void DiscardHand(Player p, params int[] ids) =>
            p.CardStatus.RemoveHandCards(ids);
        public void DiscardHandAll(Player p) =>
            DiscardHand(p, p.CardStatus.Hand.ToArray());
        public void DiscardTable(Player p, params int[] ids) =>
            p.CardStatus.RemoveTableCards(ids);
        public void DiscardTableAll(Player p) =>
            DiscardTable(p, p.CardStatus.Table.ToArray());
        public void AddTable(Player p, params int[] ids) =>
            p.CardStatus.AddTableCards(ids);

        public void AddReciverStatus(Player p, ReceiveEnum r) =>
            p.ReceiverStatus.AddReciverStatus(r);

        public void AddQuest(Player p, QuestType q)
        {
            AddQuest(p, q, new());
            p.PhaseQuestStatus.AddQuest(q);
            Action<FinishedQuestPacket> check = _ => { };
            check = packet =>
            {
                if (packet.quest != q)
                    return;

                FinishQuest(p, q);
            };
            p.PlayerStandalone.PacketHandler
                              .FinishedQuestPacketEvent
                              .AutoRemoveListener(check);
        }
        public void AddQuest(Player p, QuestType q, UnityEvent listener)
        {
            p.PhaseQuestStatus.AddQuest(q);
            listener.AutoRemoveListener(() => FinishQuest(p, q));
        }
        public void FinishQuest(Player p, QuestType q) =>
            p.PhaseQuestStatus.FinishQuest(q);

        public void UseCard(Player p, int cardId)
        {
            var function = ((CardEnum.Property)cardId).ConvertFunction();
            if (!function.ServerCheck(p, this))
                return;

            DiscardHand(p, cardId);
            var executable = function.ToExecutable();
            executable.ExecuteAction(p, this, cardId);
        }

        public void AddGameAction(GameAction action) =>
            phaseManager.Insert(new(PhaseType.Action, action));

        public void AddResolve(Action action)
        {
            if (PhaseManager.Current() is Phase_Resolving)
                (PhaseManager.Current() as Phase_Resolving).ActionStack.Push(action);
            else
                PhaseManager.Insert(PhaseType.Resolving, action);
        }

        /// <summary>
        /// 廣播訊息。
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="sendType">Default = SendType.Target</param>
        public void Broadcast(IPacket packet, SendType sendType = SendType.Target) =>
            players.Foreach(p => p.PlayerStandalone.Send(sendType, packet));
    }
}