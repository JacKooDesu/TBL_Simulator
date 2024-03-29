using Cysharp.Threading.Tasks;

using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

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

        public UnityEvent<PhaseBase> AfterEnter { get; } = new();

        public PhaseManager(Manager manager)
        {
            this.manager = manager;
            // reverse default flow for stack
            // flow = new(PHASE_FLOW.Reverse());
        }

        public void ResetFlow() => InsertRange(PHASE_FLOW.Reverse().ToArray());

        public async UniTask Run(CancellationToken ct)
        {
            while (true)
            {
                var flowCount = flow.Count;
                bool interrupt = false;
                current = Phase.Get(flow.Peek().type);
                var dt = 0f;

                current.Enter(manager, flow.Peek().parameter);
                AfterEnter.Invoke(current);
                while (current.Update(dt))
                {
                    await UniTask.Yield(PlayerLoopTiming.EarlyUpdate);
                    ct.ThrowIfCancellationRequested();
                    dt = Time.deltaTime;
                    if (flowCount != flow.Count)
                    {
                        interrupt = true;
                        break;
                    }
                }

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

        public void Insert(PhaseType type, System.Object parameter = null) => flow.Push(new(type, parameter));
        public void Insert(PhaseData data) => Insert(data.type, data.parameter);

        public void InsertRange(params PhaseData[] phaseDatas) =>
            phaseDatas.Reverse().ToList().ForEach(data => flow.Push(data));
    }
}