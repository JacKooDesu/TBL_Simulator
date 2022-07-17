using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace TBL.Hero
{
    using GameAction;

    public class HeroSkill
    {
        public string name;
        public string description;
        public bool autoActivate;
        public bool limited;
        public System.Func<CancellationTokenSource, Task<SkillAction>> localAction;    // 回傳SkillAction Command Server執行
        public System.Func<SkillAction, Task> action;    // server 端執行
        public System.Func<bool> checker;   // checker 目前僅在Server端運行檢查
        public System.Func<Task> timeoutAction;

        public HeroSkill() { }
        public HeroSkill(
            string name, string description, bool autoActivate,
            Func<CancellationTokenSource, Task<SkillAction>> localAction = null,
            Func<SkillAction, Task> action = null,
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

    }
}
