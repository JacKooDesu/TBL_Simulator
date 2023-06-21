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

        async void Start()
        {
            if (IStandaloneManager.Singleton == null)
                return;

            await UniTask.WaitUntil(() => IStandaloneManager.Singleton.InitializeComplete);

            IPlayerStandalone[] standalones = null!;
            standalones = IStandaloneManager.Singleton.GetStandalones();

            deck.Init(deckSetting)
                .sleeping.AddRange(deck.CardDatas)
                .Shuffle();

            players.Init(standalones);
            Broadcast(new ServerReadyPacket(), SendType.Target);

            await UniTask.WaitUntil(() => standalones.FirstOrDefault(x => x.IsReady == false) == null);

            players.SetupTeam(teamSetting, heroSetting, true);

            foreach (var p in players.Players)
                Draw(p, 7);

            Broadcast(new GameStartPacket(), SendType.Target);

            phaseManager = new(this);
            NewRound();
            var ct = gameObject.GetCancellationTokenOnDestroy();
            phaseManager.Run(ct).Forget();
        }

        public void NewRound()
        {
            currentPlayerIndex++;
            if (currentPlayerIndex >= players.Players.Count)
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
                p.PlayerStandalone.PacketHandler.FinishedQuestPacketEvent -= check;
            };
            p.PlayerStandalone.PacketHandler.FinishedQuestPacketEvent += check;
        }
        public void AddQuest(Player p, QuestType q, UnityEvent listener)
        {
            p.PhaseQuestStatus.AddQuest(q);
            listener.AutoRemoveListener(() => FinishQuest(p, q));
        }
        public void FinishQuest(Player p, QuestType q) =>
            p.PhaseQuestStatus.FinishQuest(q);

        /// <summary>
        /// 廣播訊息。
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="sendType">Default = SendType.Target</param>
        public void Broadcast(IPacket packet, SendType sendType = SendType.Target) =>
            players.Foreach(p => p.PlayerStandalone.Send(sendType, packet));
    }
}