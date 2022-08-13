using System.Threading.Tasks;
using System.Collections.Generic;
namespace TBL.Hero
{
    using GameActionData;
    using Util;
    public class Shining : HeroBase
    {
        protected override void BindSkill()
        {
            var skill0 = new HeroSkill
            {
                name = "狙擊",
                description = $"翻開此角色牌，然後燒毀 {RichTextHelper.TextWithBold("另一位玩家")} 面前的至多三張情報。",
                autoActivate = false,
                localAction = async (cancel) =>
                {
                    var sa = new SkillActionData(user: playerStatus.playerIndex, skill: 0);
                    var playerList = new List<int>();
                    foreach (var p in manager.players)
                    {
                        if (p.netCards.Count != 0)
                            playerList.Add(p.playerIndex);
                    }
                    netCanvas.BindSelectPlayer(playerList, index => sa.target = index);
                    await TaskExtend.WaitUntil(
                        () => sa.target != int.MinValue,
                        () => cancel.IsCancellationRequested);

                    if (cancel.IsCancellationRequested) return default;

                    return sa;
                },
                action = async (_) =>
                {
                    if (_.target == int.MinValue)
                        return false;
                    var targetPlayer = manager.players[_.target];

                    playerStatus.SetHeroState(false);
                    playerStatus.SetSkillLimited(0, true);

                    for (int i = 0; i < 3; ++i)
                    {
                        if (i != 0)
                        {
                            if (targetPlayer.netCards.Count == 0)
                                return true;
                            
                            await playerStatus.InitReturnDataMenu("燒毀情報", "取消");
                            await TaskExtend.WaitUntil(
                                () => !playerStatus.isWaitingData,
                                () => judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting);

                            if (judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting ||
                                playerStatus.tempData == 1 ||
                                playerStatus.tempData == int.MinValue)
                                return true;
                        }

                        await playerStatus.InitReturnCardMenu(_.target);
                        await TaskExtend.WaitUntil(
                            () => !playerStatus.isWaitingData,
                            () => judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting);

                        if (judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting)
                            return i != 0;

                        targetPlayer.CardTToG(_.target, playerStatus.tempData);
                        print(playerStatus.tempData);
                    }
                    return true;
                },
                checker = () =>
                {
                    if (!isHiding)
                        return false;

                    if (playerStatus.hero.skills[0].limited)
                        return false;

                    foreach (var p in manager.players)
                    {
                        if (p == playerStatus)
                            continue;
                        if (p.netCards.Count != 0)
                            return true;
                    }

                    return false;
                }
            };

            skills = new HeroSkill[] {
                skill0
            };
        }

        protected override void BindSpecialMission()
        {
            this.mission = new HeroMission(
                "親手讓另一位沒有獲得紅藍情報的玩家死亡。",
                () =>
                {

                    if (judgement.currentPhase != NetworkJudgement.Phase.Sending)
                        return false;

                    var targetPlayer = manager.players[judgement.currentSendingPlayer];
                    if (targetPlayer.isDead && judgement.currentRoundPlayerIndex == playerStatus.playerIndex)
                        return true;

                    return false;
                }
            );
        }
    }
}

