using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace TBL
{
    using Hero;
    using GameActionData;
    using UI.LogSystem;
    using Util;
    using Card;
    public partial class NetworkJudgement : NetworkBehaviour
    {
        [Server]
        public void CardAction(CardActionData ca)
        {
            manager.players[ca.user].netHandCards.Remove(ca.originCardId);
            AddCardAction(ca);
            print($"玩家({ca.user}) 對 玩家({ca.target}) 使用 {CardSetting.IdToCard(ca.cardId).GetCardNameFully()}");
        }

        public async void UseSkill(SkillActionData action)
        {
            var player = manager.players[action.user];
            var hero = player.hero;
            var skill = hero.skills[action.skill];

            ChangePhase(Phase.HeroSkillReacting);
            await TaskExtend.WaitUntil(
                () => manager.players.Find(p => p.phase == lastPhase) == null
            );
            var result = await skill.ServerUse(new ClassifyStruct<SkillActionData>(action));

            // return if action failed (maybe timeout)
            if (!result)
                return;

            manager.TargetLogAll(new LogBase(
                $"{player.playerName} ({hero.HeroName}) 使用 {skill.name}",
                true,
                false
            ));
            // will remove in future
            if (currentPhase == Phase.HeroSkillReacting)
                ChangePhase(lastPhase);
        }

        public void CheckAllHeroSkill()
        {
            for (int i = currentRoundPlayerIndex; i < manager.players.Count; ++i)
            {
                var p = manager.players[i % manager.players.Count];
                p.hero.CheckSkill();
            }
        }
    }

}
