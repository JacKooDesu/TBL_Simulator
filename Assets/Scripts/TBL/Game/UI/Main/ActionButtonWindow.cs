using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

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
            Debug.Log("Hello");
            if (status.Quest.Contains(QuestType.DrawCard))
                draw.onClick.ReBind(Draw);

            // if(status.Quest.Contains(QuestType.PassCard))
        }

        void Draw() =>
            IPlayerStandalone.Me.Send(SendType.Cmd, new FinishedQuestPacket(QuestType.DrawCard));

        void PassCard() { }

        void ResetTimer(ChangePhasePacket packet)
        {
            if (cts != null)
                cts.Cancel();
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