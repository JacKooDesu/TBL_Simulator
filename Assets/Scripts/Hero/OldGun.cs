using System.Collections.Generic;
using System.Threading.Tasks;
using TBL.UI.LogSystem;
namespace TBL.Hero
{
    using Util;
    using GameAction;
    using static Card.CardAttributeHelper;
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
                    return new SkillAction(skill: 0);
                },
                action = async (_) =>
                {
                    playerStatus.DrawCard(2);
                    playerStatus.CmdChangeHeroState(false);
                    return true;
                },
                checker = () =>
                {
                    var currentAction = manager.Judgement.currentCardAction;

                    if (manager.Judgement.currentPhase != NetworkJudgement.Phase.Reacting)
                        return false;

                    return (((Card.CardSetting)currentAction.cardId).CardType == Card.CardType.Lock ||
                            ((Card.CardSetting)currentAction.cardId).CardType == Card.CardType.Test) &&
                            currentAction.target == playerStatus.playerIndex;
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
                    var skillAction = new SkillAction(user: playerStatus.playerIndex, skill: 1);
                    netCanvas.BindSelectPlayer(
                        manager.GetOtherPlayers(),
                        (targetIndex) => skillAction.target = targetIndex
                    );

                    await TaskExtend.WaitUntil(
                        () => skillAction.target != int.MinValue,
                        () => cancel.IsCancellationRequested
                    );

                    if (cancel.IsCancellationRequested)
                        return new SkillAction();

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

            var skill3 = new HeroSkill
            {
                name = "攤牌",
                description = "翻開自己的身份牌，同時翻開另一位玩家的身份牌。",
                autoActivate = false,
                localAction = async (cancel) =>
                {
                    var skillAction = new SkillAction(user: playerStatus.playerIndex, skill: 2);

                    netCanvas.BindSelectPlayer(manager.GetOtherPlayers(), (index) => skillAction.target = index);

                    await TaskExtend.WaitUntil(
                        () => cancel.IsCancellationRequested,
                        () => skillAction.target != int.MinValue
                    );
                    // while (skillAction.target == int.MinValue && !MouseInputHandler.rightClickInvoke)
                    //     await Task.Yield();

                    if (cancel.IsCancellationRequested)
                        return new SkillAction();

                    return skillAction;
                },
                action = async (_) =>
                {
                    var target = manager.players[_.target];
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

                    return true;
                },
                checker = () =>
                {
                    if (skills[2].limited)
                        return false;

                    if (judgement.currentPhase == NetworkJudgement.Phase.Draw)
                        return false;

                    return true;
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

