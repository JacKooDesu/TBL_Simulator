using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
namespace TBL.Game.Sys
{
    public abstract class PhaseBase
    {
        protected float timeCurrent;
        protected abstract float Time { get; }
        public virtual void Enter(object parameter = null) => timeCurrent = 0;
        public virtual bool Update(float dt)
        {
            timeCurrent += dt;
            return timeCurrent < Time;
        }
        public abstract void Exit();
    }

    public enum PhaseType
    {
        Draw = 1,

    }

    public static class Phase
    {
        public static readonly ReadOnlyDictionary<PhaseType, PhaseBase> PHASE_POOL = new(
            new Dictionary<PhaseType, PhaseBase>()
            {
                {PhaseType.Draw, new Phase_Draw()},
            }
        );

        public static PhaseBase Get(PhaseType type) => PHASE_POOL[type];
    }
}
