using System.Threading.Tasks;
using System.Collections.Generic;
namespace TBL.Hero
{
    using static Card.CardAttributeHelper;
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
            skill0.localActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 選擇有黑情報的玩家
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        var playerList = new List<int>();
                        foreach(var p in  manager.players.FindAll(p=>p.GetCardCount(Black)!=0))
                            playerList.Add(p.playerIndex);

                        netCanvas.BindSelectPlayer(
                            playerList,
                            index => _.data.target = index);
                        return Task.CompletedTask;
                    },
                    checker = (_) => {
                        if(_.data.target == int.MinValue)
                            return SkillAction.CheckerState.None;

                        return SkillAction.CheckerState.Continue;
                    }
                }
            };
            skill0.serverActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 選擇黑情報
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_) => {
                        playerStatus.SetHeroState(hiding:false);
                        playerStatus.SetSkillLimited(0, true);
                        // 用suffix紀錄已燒毀數量
                        if(_.data.suffix == int.MinValue)
                            _.data.suffix = 0;

                        await playerStatus.InitReturnCardMenu(_.data.target);
                    },
                    checker = (_) => {
                        if(playerStatus.isWaitingData)
                            return SkillAction.CheckerState.None;

                        return SkillAction.CheckerState.Continue;
                    }
                },
                // 執行燒毀黑情報
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        manager.players[_.data.target].CardTToG(
                            _.data.target,
                            playerStatus.tempData
                        );
                        _.data.suffix += 1;
                        return Task.CompletedTask;
                    },
                    checker = (_) => {
                        return SkillAction.CheckerState.Continue;
                    }
                },
                // 是否繼續燒
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_) => {
                        if(manager.players[_.data.target].GetCardCount(Black) == 0)
                            _.data.suffix = 3;  // 直接變成3
                        
                        if(_.data.suffix < 3)
                            await playerStatus.InitReturnDataMenu("燒毀情報", "取消");
                    },
                    checker = (_) => {
                        if(_.data.suffix >= 3)
                            return SkillAction.CheckerState.Continue;

                        if(playerStatus.isWaitingData)
                            return SkillAction.CheckerState.None;

                        if(playerStatus.tempData == 0)
                            return SkillAction.CheckerState.Restart;
                        else
                            return SkillAction.CheckerState.Continue;
                    }
                }
            };
            skill0.commonServerBreaker = () =>
                judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting;

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

