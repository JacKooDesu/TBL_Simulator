using Cysharp.Threading.Tasks;

using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace TBL.Game.Sys
{
    public class PhaseManager
    {
        record PhaseData(PhaseType type, System.Object parameter = null);
        readonly PhaseData[] PHASE_FLOW = {
            new(PhaseType.Draw)
        };

        readonly Stack<PhaseData> flow = new();

        PhaseBase current;
        public PhaseBase Current() => current;
        public PhaseBase Next() => Phase.Get(flow.Peek().type);

        readonly Manager manager;

        public PhaseManager(Manager manager)
        {
            // reverse default flow for stack
            PHASE_FLOW.Reverse();
            flow = new(PHASE_FLOW);
        }

        void ResetFlow() => PHASE_FLOW.ToList().ForEach(p => flow.Push(p));

        public async UniTask Run(CancellationToken ct)
        {
            while (true)
            {
                var flowCount = flow.Count;
                bool interrupt = false;
                current = Phase.Get(flow.Peek().type);
                var dt = 0f;

                current.Enter();
                do
                {
                    await UniTask.Yield(PlayerLoopTiming.EarlyUpdate);
                    ct.ThrowIfCancellationRequested();
                    dt = Time.deltaTime;
                    if (flowCount != flow.Count)
                    {
                        interrupt = true;
                        break;
                    }
                } while (current.Update(dt));

                if (interrupt)
                    continue;
                else
                    flow.Pop();
            }
        }

        public void Insert(PhaseType p, System.Object parameter = null) => flow.Push(new(p, parameter));
    }
}