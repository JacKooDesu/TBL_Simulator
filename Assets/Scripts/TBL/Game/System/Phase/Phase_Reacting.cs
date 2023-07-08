using System;
using UnityEngine;

namespace TBL.Game.Sys
{
    public abstract class Phase_Reacting : PhaseBase
    {
        public record Data(ReactingType rType, Func<bool> escFunc);
        Data data { get; set; }

        protected override PhaseType PhaseType => PhaseType.Reacting;
        public enum ReactingType
        {
            Base = PhaseType.Reacting,
            ChooseTarget,
            UseCard,
            UseSkill
        }
        public ReactingType _ReactingType { get; private set; } = ReactingType.Base;
        protected override float time => 5f;

        public override string PhaseName => "反制階段";

        public override void Enter(Manager manager, object parameter = null)
        {
            data = (parameter as Data);
            base.Enter(manager);
        }

        public override bool Update(float dt) =>
        Check() & base.Update(dt);

        bool Check() => data.escFunc();

        public override void Exit()
        {

        }

        public static PhaseManager.PhaseData Create(Data data) =>
            new(PhaseType.Reacting, data);
    }
}