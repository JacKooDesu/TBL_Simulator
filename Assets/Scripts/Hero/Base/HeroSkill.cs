using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace TBL.Hero
{
    using GameActionData;
    using Util;
    public class HeroSkill
    {
        public string name;
        public string description;
        public bool autoActivate;
        public bool limited;

        public System.Func<CancellationTokenSource, Task<SkillActionData>> localAction;    // 回傳SkillAction Command Server執行
        public System.Func<SkillActionData, Task<bool>> action;    // server 端執行
        public System.Func<bool> checker;   // checker 目前僅在Server端運行檢查
        public System.Func<Task> timeoutAction;

        public HeroSkill() { }
        public HeroSkill(
            string name, string description, bool autoActivate,
            Func<CancellationTokenSource, Task<SkillActionData>> localAction = null,
            Func<SkillActionData, Task<bool>> action = null,
            Func<bool> checker = null,
            Func<Task> timeoutAction = null)
        {
            limited = false;
            this.name = name;
            this.description = description;
            this.autoActivate = autoActivate;
            this.localAction = localAction;
            this.action = action;
            this.checker = checker;
            this.timeoutAction = timeoutAction;
        }

        public SkillAction<ClassifyStruct<SkillActionData>>[] localActions;
        public Func<bool> commonLocalBreaker = () => false;
        public SkillAction<ClassifyStruct<SkillActionData>>[] serverActions;
        public Func<bool> commonServerBreaker = () => false;

        public async Task<SkillActionData> LocalUse(
            SkillActionData presetData,
            CancellationTokenSource cancellationToken)
        {
            var sa = new ClassifyStruct<SkillActionData>(presetData);
            UnityEngine.Debug.Log(sa.data.suffix);
            foreach (var a in localActions)
            {
                await a.action(sa);

                if (a.checker == null)
                    continue;

                await TaskExtend.WaitUntil(
                    () => a.checker(sa) != SkillAction<ClassifyStruct<SkillActionData>>.CheckerState.None,
                    () => commonLocalBreaker(),
                    () => cancellationToken.IsCancellationRequested);

                if (commonLocalBreaker() ||
                    a.checker(sa) == SkillAction<ClassifyStruct<SkillActionData>>.CheckerState.Break ||
                    cancellationToken.IsCancellationRequested)
                    return default(SkillActionData);
            }

            return sa.data;
        }

        public async Task<bool> ServerUse(ClassifyStruct<SkillActionData> _)
        {
            foreach (var a in serverActions)
            {
                await a.action(_);

                if (a.checker == null)
                    continue;

                await TaskExtend.WaitUntil(
                    () => a.checker(_) != SkillAction<ClassifyStruct<SkillActionData>>.CheckerState.None,
                    () => commonServerBreaker());

                if (commonServerBreaker() ||
                    a.checker(_) == SkillAction<ClassifyStruct<SkillActionData>>.CheckerState.Break)
                    return false;
            }

            return true;
        }
    }

    public class SkillAction
    {
        public Func<Task> action;
        public Func<CheckerState> checker;
        public enum CheckerState
        {
            None,
            Continue,
            Break
        }
    }

    public class SkillAction<T> : SkillAction
    {
        public new Func<T, Task> action;
        public new Func<T, CheckerState> checker;
    }
}
