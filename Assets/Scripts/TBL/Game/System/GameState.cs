using System;
using System.Linq;

namespace TBL.Game.Sys
{
    using Game.Networking;
    using UnityEngine.Events;
    using static TBL.Game.PhaseQuestStatus;

    /// <summary>
    /// 存在於所有玩家，用於紀錄當前遊戲狀態。
    /// </summary>
    public sealed class GameState : IDisposable
    {
        public static GameState Instance { get; private set; }
        public static void Create(PacketHandler handler)
        {
            Instance = new();
            Instance.RegistEvent(handler);
        }

        void RegistEvent(PacketHandler handler)
        {
            handler.ChangePhasePacketEvent.AddListener(SetPhase);
            handler.ChangePhasePacketEvent.AddListener(SetPassing);
            handler.NewRoundPacketEvent.AddListener(SetNewRound);
        }

        public PhaseType CurrentPhase { get; private set; } = PhaseType.None;
        public UnityEvent<PhaseType> OnPhaseChange { get; } = new();
        void SetPhase(ChangePhasePacket packet) =>
            OnPhaseChange.Invoke(CurrentPhase = packet.PhaseType);

        public int CurrentRoundHost { get; private set; } = -1;
        public UnityEvent<int> OnRoundHostChange { get; } = new();
        void SetNewRound(NewRoundPacket packet) =>
            OnRoundHostChange.Invoke(CurrentRoundHost = packet.HostId);

        public int? CurrnetPassingPlayer { get; private set; } = null;
        public UnityEvent<int?> OnPassingPlayerChange { get; } = new();
        void SetPassing(ChangePhasePacket packet) =>
            OnPassingPlayerChange.Invoke(
                CurrnetPassingPlayer = packet.PhaseType switch
                {
                    PhaseType.Passing =>
                        IPlayerStandalone
                            .Standalones
                            .Select(ps => ps.player)
                            .First(
                                x => x.PhaseQuestStatus
                                    .Quest
                                    .Contains(QuestType.AskRecieve))
                            .ProfileStatus
                            .Id,
                    _ => null
                });

        public void Dispose() => Instance = null;
    }
}
