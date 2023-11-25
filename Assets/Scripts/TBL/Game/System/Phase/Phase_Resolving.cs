using System;
using System.Collections.Generic;

namespace TBL.Game.Sys
{
    using Hero;
    public class Phase_Resolving : PhaseBase
    {
        protected override PhaseType PhaseType => PhaseType.Resolving;
        protected override float time => 2;

        public override string PhaseName => "效果處理";

        public Stack<ResolveDetail> ResolveStack { get; } = new();
        public ResolveDetail? GetCurrent() =>
            ResolveStack.TryPeek(out var detail) ? detail : null;
        public void Push(ResolveDetail detail) =>
            ResolveStack.Push(detail);
        public ResolveDetail? Pop() =>
            ResolveStack.TryPop(out var detail) ? detail : null;

        public override void Enter(Manager manager, object parameter = null)
        {
            if (parameter is not null && parameter is ResolveDetail)
                ResolveStack.Push((ResolveDetail)parameter);
            base.Enter(manager);
        }

        public override bool Update(float dt) =>
            Check() & base.Update(dt);

        bool Check() => true;

        public override void Exit()
        {
            while (ResolveStack.TryPop(out var detail))
            {
                detail.action(detail);
            }
        }

        public struct ResolveDetail
        {
            public enum Type
            {
                Just,
                Card,
                Skill,
                Other,
            }
            public Action<ResolveDetail> action;
            public Type type;
            public Player user;
            public Player target;
            public (HeroId hero, int id) skill;

            public static ResolveDetail Just(Action<ResolveDetail> action, Player user) =>
                new()
                {
                    type = Type.Just,
                    action = action,
                    user = user
                };

            public static ResolveDetail Card(
                Action<ResolveDetail> action,
                CardEnum.Function function,
                Player user,
                Player target = null) =>
                new()
                {
                    type = Type.Card,
                    user = user,
                    target = target,
                    action = action
                };

            public static ResolveDetail Skill(
                Action<ResolveDetail> action,
                HeroId heroId,
                int skillId,
                Player user,
                Player target = null) =>
                new()
                {
                    action = action,
                    user = user,
                    target = target,
                    skill = (heroId, skillId)
                };
        }
    }
}