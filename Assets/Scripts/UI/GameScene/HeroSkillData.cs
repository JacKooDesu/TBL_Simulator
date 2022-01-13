using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                tipStr += $"{skill.name} - {skill.description}\n";
            }

            tip.content = tipStr;
        }

        public void BindEvent(System.Action action)
        {
            JacDev.Utils.EventBinder.Bind(
                GetComponent<UnityEngine.EventSystems.EventTrigger>(),
                UnityEngine.EventSystems.EventTriggerType.PointerClick,
                action
            );
        }
    }
}

