using System.Collections.Generic;
using System.Threading.Tasks;
namespace TBL.Hero
{
    using Util;
    using GameActionData;
    using static Card.CardAttributeHelper;

    public class Killer : HeroBase
    {
        protected override void BindSkill()
        {
            var skill1 = new HeroSkill
            {
                name = "連擊",
                description = "當一位玩家獲得你傳出的黑情報時，抽兩張牌，並可以在他面前再放置一張黑情報。",
                autoActivate = true,
                action = async (_) =>
                {
                    playerStatus.DrawCard(2);
                    // 沒有黑情報可選
                    if (playerStatus.GetHandCardCount(Black) <= 0)
                        return true;

                    await playerStatus.InitReturnDataMenu("放置一張黑情報", "取消");
                    await TaskExtend.WaitUntil(
                        () => !playerStatus.isWaitingData,
                        () => judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting
                    );

                    if (judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting)
                        return true;

                    // 不放置黑情報，或是未選擇
                    if (playerStatus.tempData != 0)
                        return true;

                    playerStatus.ClearTempData();
                    await playerStatus.InitReturnHandCardMenu(playerStatus.playerIndex, Black);
                    await TaskExtend.WaitUntil(
                        () => !playerStatus.isWaitingData,
                        () => judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting
                    );

                    if (judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting)
                        return true;

                    playerStatus.CardHToT(playerStatus.tempData, judgement.currentSendingPlayer);

                    return true;
                },
                checker = () =>
                {
                    if (judgement.currentPhase != NetworkJudgement.Phase.Sending)
                        return false;

                    if (judgement.currentRoundPlayerIndex != playerStatus.playerIndex)
                        return false;

                    var targetPlayer = manager.players[judgement.currentSendingPlayer];
                    if (((Card.CardSetting)judgement.currentRoundSendingCardId).CardColor == Card.CardColor.Black)
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
                localAction = async (cancel) =>
                {
                    var sa = new SkillActionData
                    (
                        skill: 1,
                        user: playerStatus.playerIndex
                    );

                    netCanvas.ShowPlayerHandCard(
                        index: playerStatus.playerIndex,
                        action: (id) =>
                        {
                            sa.suffix = id;
                        },
                        Secret
                    );
                    await TaskExtend.WaitUntil(
                        () => sa.suffix != int.MinValue,
                        () => cancel.IsCancellationRequested);

                    netCanvas.BindSelectPlayer(
                        manager.GetOtherPlayers(),
                        (index) => sa.target = index
                    );
                    await TaskExtend.WaitUntil(
                        () => sa.target != int.MinValue,
                        () => cancel.IsCancellationRequested);

                    return sa;
                },
                action = async (_) =>
                {
                    judgement.ChangePhase(judgement.lastPhase);
                    judgement.AddCardAction(
                        new CardActionData
                        {
                            cardId = Lock,
                            target = _.target,
                            user = playerStatus.playerIndex
                        });
                    playerStatus.CardHToG(_.suffix);

                    return true;
                },
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
                localAction = async (cancel) =>
                {
                    var sa = new SkillActionData
                    (
                        skill: 1,
                        user: playerStatus.playerIndex
                    );

                    netCanvas.ShowPlayerHandCard(
                        index: playerStatus.playerIndex,
                        action: (id) =>
                        {
                            sa.suffix = id;
                        }
                    );
                    await TaskExtend.WaitUntil(
                        () => sa.suffix != int.MinValue,
                        () => cancel.IsCancellationRequested);

                    netCanvas.BindSelectPlayer(
                        manager.GetOtherPlayers(),
                        (index) => sa.target = index
                    );
                    await TaskExtend.WaitUntil(
                        () => sa.target != int.MinValue,
                        () => cancel.IsCancellationRequested);

                    return sa;
                },
                action = async (_) =>
                {
                    manager.players[_.target].isLocked = true;
                    playerStatus.CardHToG(_.suffix);

                    return true;
                },
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

