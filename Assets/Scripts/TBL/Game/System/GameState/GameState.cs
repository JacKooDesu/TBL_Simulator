using System;
using System.Linq;

namespace TBL.Game.Sys
{
    using Game.Networking;
    using TBL.Utils;
    using UnityEngine.Events;
    using static TBL.Game.PhaseQuestStatus;

    /// <summary>
    /// 存在於所有玩家，用於紀錄當前遊戲狀態。
    /// </summary>
    public sealed partial class GameState : IDisposable
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
            handler.NewRoundPacketEvent.AddListener(SetNewRound);
            OnPhaseChange.AddListener(OnPhaseChangeAction);
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

        void OnPhaseChangeAction(PhaseType packet)
        {
            Action action = packet switch
            {
                PhaseType.Passing => SetPassing,
                PhaseType.Receive => SetReceive,
                _ => () => { }
            };
            action.Invoke();
        }

        void SetPassingPlayer(int? id)
        {
            CurrnetPassingPlayer = id;
            OnPassingPlayerChange.Invoke(CurrnetPassingPlayer);
        }

        void SetPassing()
        {
            foreach (var p in IPlayerStandalone.Standalones.Select(x => x.player))
            {
                UnityAction<PhaseQuestStatus> action = s =>
                {
                    if (!s.Quest.Contains(QuestType.AskRecieve))
                        return;
                    SetPassingPlayer(p.ProfileStatus.Id);
                };
                p.PhaseQuestStatus.OnChanged.AddListener(action);

                E_Phase_Recive.AutoRemoveListener(
                    () => p.PhaseQuestStatus.OnChanged.RemoveListener(action));
            }
        }
        private void SetReceive()
        {
            E_Phase_Recive.Invoke();
        }

        public void Dispose() => Instance = null;
    }
}
