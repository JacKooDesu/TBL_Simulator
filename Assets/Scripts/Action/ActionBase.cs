using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.Action
{
    [System.Serializable]
    public struct CardAction
    {
        public int user;
        public int target;
        public int cardId;
        public int originCardId;
        public int suffix;

        public CardAction(int user, int target, int cardId, int originCardId, int suffix)
        {
            this.user = user;
            this.target = target;
            this.cardId = cardId;
            this.originCardId = originCardId;
            this.suffix = suffix;
        }
    }

    public struct SkillAction
    {
        public int user;
        public int target;
        public int skill;
        public int suffix;
        public SkillAction(int user, int target, int skill, int suffix)
        {
            this.user = user;
            this.target = target;
            this.skill = skill;
            this.suffix = suffix;
        }
    }
}
