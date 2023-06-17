using System.Collections.Generic;
using System.Threading.Tasks;
using TBL.UI.LogSystem;
namespace TBL.Game.Hero
{
    using Judgement;
    using Util;
    using GameActionData;
    using static ObsleteCard.CardAttributeHelper;
    public class OldGun : HeroBase
    {
        protected override void BindSkill()
        {
            // 就計
            // 當你被試探或鎖定時，可以翻開此角色，然後抽兩張牌。
            var skill1 = new HeroSkill
            {
                name = "就計",
                description = "當你被試探或鎖定時，可以翻開此角色，然後抽兩張牌。",
                autoActivate = false,
                localAction = async (cancel) =>
                {
                    await Task.CompletedTask;
                    return new SkillActionData(skill: 0);
                },
                // action = async (_) =>
                // {
                //     playerStatus.DrawCard(2);
                //     playerStatus.CmdChangeHeroState(false);
                //     return true;
                // },
                checker = () =>
                {
                    var currentAction = manager.Judgement.currentCardAction;

                    if (manager.Judgement.currentPhase != NetworkJudgement.Phase.Reacting)
                        return false;

                    return (((ObsleteCard.CardSetting)currentAction.cardId).CardType == ObsleteCard.CardType.Lock ||
                            ((ObsleteCard.CardSetting)currentAction.cardId).CardType == ObsleteCard.CardType.Test) &&
                            currentAction.target == playerStatus.playerIndex;
                }
            };
            skill1.localActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 被鎖定時發動直接回傳 ActionData
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        return Task.CompletedTask;
                    }
                }
            };
            skill1.serverActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 抽二，翻開角色
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        playerStatus.DrawCard(2);
                        playerStatus.SetHeroState(hiding:false);
                        return Task.CompletedTask;
                    }
                }
            };

            // 合謀
            // 蓋伏你的角色牌並和另一位角色互看身分牌，如果對方是未蓋伏的潛伏角色，你可以選擇蓋伏對方的角色牌並放置一張黑情報在面前
            var skill2 = new HeroSkill
            {
                name = "合謀",
                description = "蓋伏你的角色牌並和另一位角色互看身分牌，如果對方是未蓋伏的潛伏角色，你可以選擇蓋伏對方的角色牌並放置一張黑情報在面前。",
                autoActivate = false,
                localAction = async (cancel) =>
                {
                    var skillAction = new SkillActionData(user: playerStatus.playerIndex, skill: 1);
                    netCanvas.BindSelectPlayer(
                        manager.GetOtherPlayers(),
                        (targetIndex) => skillAction.target = targetIndex
                    );

                    await TaskExtend.WaitUntil(
                        () => skillAction.target != int.MinValue,
                        () => cancel.IsCancellationRequested
                    );

                    if (cancel.IsCancellationRequested)
                        return new SkillActionData();

                    return skillAction;
                },
                action = async (_) =>
                {
                    var target = manager.players[_.target];
                    manager.RpcLog(
                        new LogBase(
                            $"玩家 {playerStatus.playerIndex} ({playerStatus.playerName}) 為 {playerStatus.Team.name} 陣營",
                            true,
                            true,
                            _.target),
                        target
                    );

                    manager.RpcLog(
                        new LogBase(
                            $"玩家 {target.playerIndex} ({target.playerName}) 為 {target.Team.name} 陣營",
                            true,
                            true,
                            new int[] { playerStatus.playerIndex }),
                        playerStatus
                    );

                    var targetHero = manager.players[_.target].hero;
                    if (targetHero.HeroType != HeroType.Hidden || targetHero.isHiding)
                        return true;

                    if (playerStatus.GetHandCardCount(Black) == 0)
                        return true;

                    await playerStatus.InitReturnDataMenu(
                        "蓋伏對方角色，並放置一張黑情報在面前",
                        "取消"
                    );

                    await TaskExtend.WaitUntil(
                        () => !playerStatus.isWaitingData,
                        () => judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting
                    );

                    if (judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting)
                        return true;

                    if (playerStatus.tempData == 1)
                        return true;

                    await playerStatus.InitReturnHandCardMenu(
                        playerStatus.playerIndex, Black
                    );

                    await TaskExtend.WaitUntil(
                        () => !playerStatus.isWaitingData,
                        () => judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting
                    );

                    if (judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting)
                        return true;

                    manager.players[_.target].AddCard(playerStatus.tempData);
                    return true;
                },
                checker = () =>
                {
                    if (isHiding)
                        return false;

                    if (judgement.currentPhase == NetworkJudgement.Phase.Draw)
                        return false;

                    return true;
                }
            };
            skill2.localActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 選擇另一個玩家
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        netCanvas.BindSelectPlayer(
                            manager.GetOtherPlayers(),
                            (index) => _.data.target = index
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
            skill2.serverActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // Log雙方身分，檢查對方是否為為蓋伏的潛伏腳色
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        var target = manager.players[_.data.target];
                        manager.RpcLog(
                            new LogBase(
                                $"玩家 {playerStatus.playerIndex} ({playerStatus.playerName}) 為 {playerStatus.Team.name} 陣營",
                                true,
                                true,
                                _.data.target
                            ),
                            target
                        );
                        manager.RpcLog(
                            new LogBase(
                                $"玩家 {_.data.target} ({target.playerName}) 為 {target.Team.name} 陣營",
                                true,
                                true,
                                new int[] { playerStatus.playerIndex }
                            ),
                            playerStatus
                        );
                        return Task.CompletedTask;
                    },
                    checker = (_) => {
                        var targetHero = manager.players[_.data.target].hero;
                        if(targetHero.HeroType != HeroType.Hidden || targetHero.isHiding)
                            return SkillAction.CheckerState.Finish;

                        if(playerStatus.GetHandCardCount(Black) == 0)
                            return SkillAction.CheckerState.Finish;

                        return SkillAction.CheckerState.Continue;
                    }
                },
                // 詢問是否放置黑情報並蓋伏對方腳色
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_) => {
                        await playerStatus.InitReturnDataMenu(
                            "蓋伏對方角色，並放置一張黑情報在面前",
                            "取消"
                        );
                    },
                    checker = (_) => {
                        if(playerStatus.isWaitingData)
                            return SkillAction.CheckerState.None;

                        if(playerStatus.tempData == 0)
                            return SkillAction.CheckerState.Continue;
                        else
                            return SkillAction.CheckerState.Finish;
                    }
                },
                // 選擇黑情報
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_) => {
                        await playerStatus.InitReturnHandCardMenu(
                            playerStatus.playerIndex,
                            Black
                        );
                    },
                    checker = (_) => {
                        if(playerStatus.isWaitingData)
                            return SkillAction.CheckerState.None;

                        return SkillAction.CheckerState.Continue;
                    }
                },
                // 放置黑情報動作
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        manager.players[_.data.target].AddCard(playerStatus.tempData);
                        return Task.CompletedTask;
                    }
                }
            };
            skill2.commonServerBreaker = () =>
                judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting;

            // 攤牌
            // 翻開自己的身份牌，同時翻開另一位玩家的身份牌。
            var skill3 = new HeroSkill
            {
                name = "攤牌",
                description = "翻開自己的身份牌，同時翻開另一位玩家的身份牌。",
                autoActivate = false,
                localAction = async (cancel) =>
                {
                    var skillAction = new SkillActionData(user: playerStatus.playerIndex, skill: 2);

                    netCanvas.BindSelectPlayer(manager.GetOtherPlayers(), (index) => skillAction.target = index);

                    await TaskExtend.WaitUntil(
                        () => cancel.IsCancellationRequested,
                        () => skillAction.target != int.MinValue
                    );
                    // while (skillAction.target == int.MinValue && !MouseInputHandler.rightClickInvoke)
                    //     await Task.Yield();

                    if (cancel.IsCancellationRequested)
                        return new SkillActionData();

                    return skillAction;
                },
                // action = async (_) =>
                // {
                //     var target = manager.players[_.target];
                //     manager.TargetLogAll(
                //         new LogBase(
                //             $"玩家 {playerStatus.playerIndex} ({playerStatus.playerName}) 為 {playerStatus.Team.name} 陣營",
                //             true,
                //             false)
                //     );

                //     manager.TargetLogAll(
                //         new LogBase(
                //             $"玩家 {target.playerIndex} ({target.playerName}) 為 {target.Team.name} 陣營",
                //             true,
                //             false)
                //     );

                //     return true;
                // },
                checker = () =>
                {
                    if (skills[2].limited)
                        return false;

                    if (judgement.currentPhase == NetworkJudgement.Phase.Draw)
                        return false;

                    return true;
                }
            };
            skill3.localActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 選擇對象
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        netCanvas.BindSelectPlayer(
                            manager.GetOtherPlayers(),
                            (index) => _.data.target = index
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
                // Log 玩家資訊
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        playerStatus.SetSkillLimited(2, true);
                        
                        var target = manager.players[_.data.target];
                        manager.TargetLogAll(
                            new LogBase(
                                $"玩家 {playerStatus.playerIndex} ({playerStatus.playerName}) 為 {playerStatus.Team.name} 陣營",
                                true,
                                false)
                        );

                        manager.TargetLogAll(
                            new LogBase(
                                $"玩家 {target.playerIndex} ({target.playerName}) 為 {target.Team.name} 陣營",
                                true,
                                false)
                        );
                        return Task.CompletedTask;
                    }
                }
            };

            skills = new HeroSkill[]{
                skill1,skill2,skill3
            };
        }

        protected override void BindSpecialMission()
        {
            // mission = new HeroMission(
            //     $"獲得三張或以上 {RichTextGeneral.red} 情報",
            //     () =>
            //     {
            //         return playerStatus.GetCardCount(Card.CardColor.Red) >= 3;
            //     }
            // );
        }
    }
}

