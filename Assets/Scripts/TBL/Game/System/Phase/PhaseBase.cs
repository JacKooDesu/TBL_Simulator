using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
namespace TBL.Game.Sys
{
    using Networking;
    public abstract class PhaseBase
    {
        protected abstract PhaseType PhaseType { get; }
        public PhaseType Type() => PhaseType;
        public abstract string PhaseName { get; }
        protected float timeCurrent;
        protected abstract float time { get; }
        public float Time => time;

        protected Manager manager;
        protected bool forceExit = false;
        public virtual void Enter(Manager manager, object parameter = null)
        {
            this.manager = manager;
            manager.Broadcast(new ChangePhasePacket(PhaseType));
            timeCurrent = 0;
            forceExit = false;
        }

        public virtual bool Update(float dt)
        {
            timeCurrent += dt;
            return timeCurrent < time & !forceExit;
        }
        public abstract void Exit();
    }

    public enum PhaseType
    {
        None = -1,

        Draw = 1,
        Main,
        Passing,
        Receive,
        End,

        Action,

        Reacting = 100,
        Resolving = 101,
    }

    public static class Phase
    {
        public static readonly ReadOnlyDictionary<PhaseType, PhaseBase> PHASE_POOL = new(
            new Dictionary<PhaseType, PhaseBase>()
            {
                {PhaseType.Draw, new Phase_Draw()},
                {PhaseType.Passing, new Phase_Passing()},
                {PhaseType.End, new Phase_End()},
                {PhaseType.Main, new Phase_Main()},
                {PhaseType.Receive, new Phase_Recive()},
                {PhaseType.Action, new Phase_Action()},
                {PhaseType.Reacting, new Phase_Reacting()},
                {PhaseType.Resolving, new Phase_Resolving()}
            }
        );

        public static PhaseBase Get(PhaseType type) => PHASE_POOL[type];
    }
}
