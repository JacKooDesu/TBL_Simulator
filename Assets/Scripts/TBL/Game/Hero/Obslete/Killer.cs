using System.Collections.Generic;
using System.Threading.Tasks;
namespace TBL.Game.Hero
{
    using Judgement;
    using Util;
    using GameActionData;
    using static ObsleteCard.CardAttributeHelper;

    public class Killer : HeroBase
    {
        protected override void BindSkill()
        {
            var skill1 = new HeroSkill
            {
                name = "連擊",
                description = "當一位玩家獲得你傳出的黑情報時，抽兩張牌，並可以在他面前再放置一張黑情報。",
                autoActivate = true,
                checker = () =>
                {
                    if (judgement.currentPhase != NetworkJudgement.Phase.Sending)
                        return false;

                    if (judgement.currentRoundPlayerIndex != playerStatus.playerIndex)
                        return false;

                    var targetPlayer = manager.players[judgement.currentSendingPlayer];
                    if (((ObsleteCard.CardSetting)judgement.currentRoundSendingCardId).CardColor == ObsleteCard.CardColor.Black)
                        return true;

                    return false;
                }
            };
            skill1.serverActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 抽二，詢問是否放置黑情報
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_)=> {
                        playerStatus.DrawCard(2);
                        if (playerStatus.GetHandCardCount(Black) > 0)
                            await playerStatus.InitReturnDataMenu("放置一張黑情報", "取消");
                    },
                    checker = (_)=> {
                        if (playerStatus.GetHandCardCount(Black) <= 0)
                            return SkillAction.CheckerState.Break;

                        if(playerStatus.isWaitingData)
                            return SkillAction.CheckerState.None;

                        if(playerStatus.tempData == 0)
                            return SkillAction.CheckerState.Continue;
                        else
                            return SkillAction.CheckerState.Break;
                    }
                },
                // 選擇放置黑情報
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_)=>{
                        await playerStatus.InitReturnHandCardMenu(playerStatus.playerIndex, Black);
                    },
                    checker = (_)=>{
                        if(playerStatus.isWaitingData)
                            return SkillAction<ClassifyStruct<SkillActionData>>.CheckerState.None;

                        return SkillAction<ClassifyStruct<SkillActionData>>.CheckerState.Continue;
                    }
                },
                // 放置黑情報
                new SkillAction<ClassifyStruct<SkillActionData>>{
                    action = (_)=>{
                        playerStatus.CardHToT(playerStatus.tempData, judgement.currentSendingPlayer);
                        return Task.CompletedTask;
                    },
                    checker = (_)=> SkillAction.CheckerState.Continue
                }
            };
            skill1.commonServerBreaker = () =>
                judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting;

            // WIP
            var skill2 = new HeroSkill
            {
                name = "預謀",
                description = $"你的 {RichTextHelper.TextWithBold("密電")} 可以當作 {RichTextHelper.TextWithBold("鎖定")} 使用。",
                autoActivate = false,
                checker = () =>
                {
                    if (playerStatus.GetHandCardCount(Secret) == 0)
                        return false;

                    if (judgement.currentRoundPlayerIndex != playerStatus.playerIndex)
                        return false;

                    if (judgement.currentPhase != NetworkJudgement.Phase.ChooseToSend)
                        return false;

                    return true;
                }
            };
            skill2.localActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 選擇密電當作鎖定
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_)=>{
                        netCanvas.ShowPlayerHandCard(
                            index: playerStatus.playerIndex,
                            action: (id) =>
                            {
                                _.data.suffix = id;
                            },
                            Secret
                        );
                        return Task.CompletedTask;
                    },
                    checker = (_)=>{
                        if(_.data.suffix == int.MinValue)
                            return SkillAction.CheckerState.None;

                        return SkillAction.CheckerState.Continue;
                    }
                },
                // 選擇鎖定玩家
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action =(_)=>{
                        netCanvas.BindSelectPlayer(
                            manager.GetOtherPlayers(),
                            (index) => _.data.target = index
                        );
                        return Task.CompletedTask;
                    },
                    checker=(_)=>{
                        if(_.data.target == int.MinValue)
                            return SkillAction.CheckerState.None;

                        return SkillAction.CheckerState.Continue;
                    }
                }
            };
            skill2.serverActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 新增鎖定動作
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_)=>{
                        judgement.ChangePhase(judgement.lastPhase);
                        judgement.AddCardAction(
                            new CardActionData
                            {
                                cardId = Lock,
                                target = _.data.target,
                                user = playerStatus.playerIndex
                            });
                        playerStatus.CardHToG(_.data.suffix);
                        return Task.CompletedTask;
                    }
                }
            };

            var skill3 = new HeroSkill
            {
                name = "冷血",
                description = $"可將任意手牌當作 {RichTextHelper.TextWithBold("鎖定")} 使用，且無法被識破。",
                autoActivate = false,
                checker = () =>
                {
                    if (playerStatus.GetHandCardCount(Secret) == 0)
                        return false;

                    if (judgement.currentRoundPlayerIndex != playerStatus.playerIndex)
                        return false;

                    if (judgement.currentPhase != NetworkJudgement.Phase.ChooseToSend)
                        return false;

                    return true;
                }
            };
            skill3.localActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 選擇手牌
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_)=>{
                        netCanvas.ShowPlayerHandCard(
                            playerStatus.playerIndex,
                            (id) => _.data.suffix = id
                        );
                        return Task.CompletedTask;
                    },
                    checker = (_)=>{
                        if(_.data.suffix == int.MinValue)
                            return SkillAction.CheckerState.None;

                        return SkillAction.CheckerState.Continue;
                    }
                },
                // 選擇對象
                new SkillAction<ClassifyStruct<SkillActionData>>{
                    action = (_) => {
                        netCanvas.BindSelectPlayer(
                            manager.GetOtherPlayers(),
                            (index) => _.data.target=index
                        );
                        return Task.CompletedTask;
                    },
                    checker = (_) => {
                        if(_.data.target == int.MinValue)
                            return SkillAction.CheckerState.None;

                        return SkillAction.CheckerState.Continue;
                    }
                }
            };
            skill3.serverActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 直接鎖定，無法識破
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        manager.players[_.data.target].isLocked = true;
                        playerStatus.CardHToG(_.data.suffix);
                        return Task.CompletedTask;
                    }
                }
            };

            skills = new HeroSkill[] { skill1, skill2, skill3 };
        }

        protected override void BindSpecialMission()
        {
            mission = new HeroMission(
                "親手讓另一位玩家成為第二位或以後死亡的玩家。",
                () =>
                {
                    if (judgement.currentPhase != NetworkJudgement.Phase.Sending)
                        return false;

                    var targetPlayer = manager.players[judgement.currentSendingPlayer];
                    if (targetPlayer.isDead && judgement.currentRoundPlayerIndex == playerStatus.playerIndex && manager.GetDeadPlayerCount() >= 2)
                        return true;

                    return false;
                }
            );
        }
    }
}

