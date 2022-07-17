using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace TBL
{
    using Hero;
    using GameAction;
    using UI.LogSystem;
    public partial class NetworkJudgement : NetworkBehaviour
    {
        public async void UseSkill(SkillAction action)
        {
            var player = manager.players[action.user];
            var hero = player.hero;
            var skill = hero.skills[action.skill];

            ChangePhase(Phase.HeroSkillReacting);
            await skill.action(action);
            manager.TargetLogAll(new LogBase(
                $"{player.playerName} ({hero.HeroName}) 使用 {skill.name}",
                true,
                false
            ));

            ChangePhase(lastPhase);
        }
    }

}
