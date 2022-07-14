using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TBL.Hero
{
    using GameAction;

    public class HeroSkill
    {
        public string name;
        public string description;
        public bool autoActivate;
        public bool limited;
        public System.Func<Task<SkillAction>> localAction;    // 回傳SkillAction Command Server執行
        public System.Action<SkillAction> action;    // server 端執行
        public System.Func<bool> checker;   // checker 目前僅在Server端運行檢查
        public System.Action timeoutAction;

        public HeroSkill() { }
        public HeroSkill(
            string name, string description, bool autoActivate,
            Func<Task<SkillAction>> localAction = null,
            Action<SkillAction> action = null,
            Func<bool> checker = null,
            Action timeoutAction = null)
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
