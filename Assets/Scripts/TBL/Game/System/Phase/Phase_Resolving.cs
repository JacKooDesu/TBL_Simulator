using System;
using System.Collections.Generic;

namespace TBL.Game.Sys
{
    public class Phase_Resolving : PhaseBase
    {
        protected override PhaseType PhaseType => PhaseType.Resolving;
        protected override float time => 2;

        public override string PhaseName => "效果處理";

        public Stack<Action> ActionStack { get; } = new();

        public override void Enter(Manager manager, object parameter = null)
        {
            if(parameter != null)
                ActionStack.Push(parameter as Action);
            base.Enter(manager);
        }

        public override bool Update(float dt) =>
            Check() & base.Update(dt);

        bool Check() => true;

        public override void Exit()
        {
            while (ActionStack.TryPop(out var action))
            {
                action();
            }
        }
    }
}