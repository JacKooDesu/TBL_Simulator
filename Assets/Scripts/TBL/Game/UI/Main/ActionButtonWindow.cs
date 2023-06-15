using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace TBL.Game.UI.Main
{
    using UI;
    using Sys;
    using Utils;
    using Networking;
    using QuestType = PhaseQuestStatus.QuestType;
    public class ActionButtonWindow : Window, ISetupWith<IPlayerStandalone>
    {
        [SerializeField] Button draw;
        [SerializeField] Button pass;
        [SerializeField] Button accept;
        [SerializeField] Button reject;
        [SerializeField] Button use;

        float time;
        [SerializeField] Text timerText;
        CancellationTokenSource cts;

        public void Setup(IPlayerStandalone res)
        {
            if (res != IPlayerStandalone.Me)
                return;

            res.player.PhaseQuestStatus.OnChanged += BindEvent;
            res.PacketHandler.ChangePhasePacketEvent += ResetTimer;
        }

        void BindEvent(PhaseQuestStatus status)
        {
            if (status.Quest.Contains(QuestType.DrawCard))
            {
                draw.onClick.ReBind(Draw);
                draw.interactable = true;
            }
            else
                draw.interactable = false;


            MainUIManager.Singleton.OnChangeSelectCard.RemoveListener(UpdatePassBtnState);
            if (status.Quest.Contains(QuestType.PassCard))
            {
                MainUIManager.Singleton.OnChangeSelectCard.AddListener(UpdatePassBtnState);
                UpdatePassBtnState(0);
            }
        }

        void Draw() =>
            IPlayerStandalone.Me.Send(SendType.Cmd, new FinishedQuestPacket(QuestType.DrawCard));

        void UpdatePassBtnState(CardEnum.Property card)
        {
            pass.interactable = card != 0;
            Predicate<ProfileStatus> canPassCheck;
            if (card.HasFlag(CardEnum.Property.Direct))
                canPassCheck = s => s.Id != IPlayerStandalone.MyPlayer.ProfileStatus.Id;
            else
            {
                var count = IPlayerStandalone.Standalones.Count - 1;
                var myId = IPlayerStandalone.Me.player.ProfileStatus.Id;
                var next = myId + 1 > count ? 0 : myId + 1;
                var last = myId - 1 < 0 ? count : myId - 1;
                canPassCheck = s =>
                    s.Id == next ||
                    s.Id == last;
            }

            pass.onClick.ReBind(() =>
                MainUIManager.Singleton.PlayerListWindow.EnterPlayerSelect(
                    p => canPassCheck(p.player.ProfileStatus),
                    target => IPlayerStandalone.Me.Send<PassCardPacket>(SendType.Cmd, new(card, target))),
                    true);
        }
        void PassCard() { }

        void ResetTimer(ChangePhasePacket packet)
        {
            if (cts != null)
                cts.Cancel(true);
            cts = new();
            var ct = gameObject.GetCancellationTokenOnDestroy();
            cts.AddTo(ct);

            time = Phase.Get(packet.PhaseType).Time;

            UpdateTime(ct).Forget();
        }

        async UniTask UpdateTime(CancellationToken ct)
        {
            while (true)
            {
                await UniTask.Delay(1000, cancellationToken: ct);
                ct.ThrowIfCancellationRequested();
                time -= 1;
                timerText.text = $"{time}";
            }
        }
    }
}