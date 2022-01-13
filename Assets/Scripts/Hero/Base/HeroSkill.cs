using System;
using System.Collections.Generic;

namespace TBL
{
    public class HeroSkill
    {
        public string name;
        public string description;
        public bool autoActivate;
        public bool limited;
        public System.Action action;
        public System.Func<bool> checker;   // checker 目前僅在Server端運行檢查

        public HeroSkill() { }
        public HeroSkill(string name, string description, bool autoActivate, System.Action action = null, System.Func<bool> checker = null)
        {
            limited = false;
            this.name = name;
            this.description = description;
            this.autoActivate = autoActivate;
            this.action = action;
            this.checker = checker;
        }

    }
}
