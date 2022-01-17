using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TBL.UI.GameScene
{
    public class HeroSkillData : MonoBehaviour
    {
        public Animator animator;
        public void Init(Hero hero)
        {
            animator = GetComponent<Animator>();

            var tip = GetComponent<TipTrigger>();
            string tipStr = "";

            foreach (var skill in hero.skills)
            {
                tipStr += $"{RichTextHelper.TextWithBold(skill.name)} - {skill.description}\n";
            }

            tip.content = tipStr;
        }

        public void BindEvent(System.Action action)
        {
            var trigger = GetComponent<EventTrigger>();

            ClearEvent(EventTriggerType.PointerClick);

            JacDev.Utils.EventBinder.Bind(
                trigger,
                UnityEngine.EventSystems.EventTriggerType.PointerClick,
                action
            );
        }

        public void ClearEvent(EventTriggerType type)
        {
            var trigger = GetComponent<EventTrigger>();

            if (trigger.triggers.Find((e) => e.eventID == type) != null)
                trigger.triggers.RemoveAll((e) => e.eventID == type);
        }
    }
}

