using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
namespace TBL.Game.Sys
{
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
        int currentPlayerIndex = 0;
        public Player CurrentPlayer => Players[currentPlayerIndex];

        PhaseManager phaseManager;

        async void Start()
        {
            if ((NetworkManager.singleton != null &&
                (NetworkManager.singleton.Me() != null && NetworkManager.singleton.Me().isClient)) ||
                LocalManager.Singleton == null)
                return;

            IPlayerStandalone[] standalones = null!;
            if (NetworkManager.singleton)
            {
                standalones = NetworkManager.singleton.players.ToArray();
            }
            else if (LocalManager.Singleton)
            {
                await UniTask.WaitUntil(() => LocalManager.Singleton.InitComplete);
                standalones = LocalManager.Singleton.Players.ToArray();
            }

            deck.Init(deckSetting)
                .sleeping.AddRange(deck.CardDatas)
                .Shuffle();
            players.Init(teamSetting, heroSetting, standalones);

            foreach (var p in players.Players)
                Draw(p, 7);

            currentPlayerIndex = 0;

            phaseManager = new(this);
            var ct = gameObject.GetCancellationTokenOnDestroy();
            phaseManager.Run(ct).Forget();
        }

        public void Draw(Player p, int count) =>
            p.CardStatus.AddHandCards(deck.Draw(count).ToIdList().ToArray());
        public void Draw(int id, int count) => Draw(Players.QueryById(id), count);

        public void AddQuest(Player p, QuestType q)
        {
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
        public void FinishQuest(Player p, QuestType q) =>
            p.PhaseQuestStatus.FinishQuest(q);

        /// <summary>
        /// 廣播訊息，使用 Target RPC。
        /// </summary>
        public void Broadcast(IPacket packet) =>
            players.Foreach(p => p.PlayerStandalone.Send(SendType.Target, packet));
    }
}