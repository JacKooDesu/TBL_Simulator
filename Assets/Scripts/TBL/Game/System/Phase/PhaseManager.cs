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
        public record PhaseData(PhaseType type, System.Object parameter = null);
        readonly PhaseData[] PHASE_FLOW = {
            // new(PhaseType.Draw),
            new(PhaseType.Main),
        };

        readonly Stack<PhaseData> flow = new();

        PhaseBase current;
        public PhaseBase Current() => current;
        public PhaseBase Next() => Phase.Get(flow.Peek().type);

        readonly Manager manager;

        public PhaseManager(Manager manager)
        {
            this.manager = manager;
            // reverse default flow for stack
            flow = new(PHASE_FLOW.Reverse());
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

                current.Enter(manager, flow.Peek().parameter);
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
                {
                    flow.Pop();
                    current.Exit();
                }

                if (flow.Count == 0)
                {
                    Debug.Log("Phase manager finished all stacks!");
                    break;
                }
            }
        }

        public void Insert(PhaseType p, System.Object parameter = null) => flow.Push(new(p, parameter));

        public void InsertRange(params PhaseData[] phaseDatas) =>
            phaseDatas.Reverse().ToList().ForEach(data => flow.Push(data));
    }
}