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
        public bool limited = false;
        public bool roundLimited = false;   // 同一輪不能重複使用

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
            for (int i = 0; i < localActions.Length; ++i)
            {
                var a = localActions[i];
                var checkerResult = SkillAction.CheckerState.None;

                await a.action(sa);

                if (a.checker == null)
                    continue;

                await TaskExtend.WaitUntil(
                    () => (checkerResult = a.checker(sa)) != SkillAction.CheckerState.None,
                    () => commonLocalBreaker(),
                    () => cancellationToken.IsCancellationRequested);

                if (checkerResult == SkillAction.CheckerState.Redo)
                    --i;

                if (checkerResult == SkillAction.CheckerState.Restart)
                    i = -1;

                if (checkerResult == SkillAction.CheckerState.Finish)
                    break;

                if (commonLocalBreaker() ||
                    checkerResult == SkillAction.CheckerState.Break ||
                    cancellationToken.IsCancellationRequested)
                    return default(SkillActionData);
            }

            return sa.data;
        }

        public async Task<bool> ServerUse(ClassifyStruct<SkillActionData> _)
        {
            roundLimited = true;
            for (int i = 0; i < serverActions.Length; ++i)
            {
                var a = serverActions[i];
                var checkerResult = SkillAction.CheckerState.None;

                await a.action(_);

                if (a.checker == null)
                    continue;

                await TaskExtend.WaitUntil(
                    () => (checkerResult = a.checker(_)) != SkillAction.CheckerState.None,
                    () => commonServerBreaker());

                if (checkerResult == SkillAction.CheckerState.Redo)
                    --i;

                if (checkerResult == SkillAction.CheckerState.Restart)
                    i = -1;

                if (checkerResult == SkillAction.CheckerState.Finish)
                    break;

                if (commonServerBreaker() ||
                    checkerResult == SkillAction.CheckerState.Break)
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
            Break,
            Redo,
            Restart,
            Finish
        }
    }

    public class SkillAction<T> : SkillAction
    {
        public new Func<T, Task> action;
        public new Func<T, CheckerState> checker;
    }
}
