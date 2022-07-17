using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

namespace TBL
{
    using Hero;
    using GameAction;
    using UI.LogSystem;
    public partial class NetworkJudgement : NetworkBehaviour
    {
        [Command]
        public async void CmdUseSkill(SkillAction action)
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
