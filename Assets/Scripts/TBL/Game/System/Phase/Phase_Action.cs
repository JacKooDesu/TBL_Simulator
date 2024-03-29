using TBL.Game.Networking;
using TBL.Utils;

namespace TBL.Game.Sys
{
    public class Phase_Action : PhaseBase
    {
        protected override PhaseType PhaseType => PhaseType.Action;
        protected override float time => 8;

        public override string PhaseName => "動作階段";

        GameAction action;
        bool complete = false;

        public override void Enter(Manager manager, object parameter = null)
        {
            action = parameter as GameAction;

            complete = false;

            var standalone = action.Player.PlayerStandalone;
            standalone.PacketHandler
                      .ActionResponsePacketEvent
                      .AutoRemoveListener(GetResponse);
            standalone.Send(SendType.Target, action.RequestCreate());
            base.Enter(manager);
        }

        void GetResponse(ActionResponsePacket packet)
        {
            action.SetResponse(packet);
            complete = true;
        }

        public override bool Update(float dt) =>
            Check() & base.Update(dt);

        bool Check() => !complete;

        public override void Exit()
        {
            if (complete) action.CompleteCallback.Invoke(action.Result);
            else action.DiscardCallback.Invoke();
            action = null;
        }
    }
}