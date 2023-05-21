using System.Threading.Tasks;

namespace TBL.Game.Hero
{
    using Judgement;
    using Util;
    using GameActionData;
    using static Card.CardAttributeHelper;
    public class SevenHundreds : HeroBase
    {
        protected override void BindSkill()
        {
            // 敏捷
            // 可以在他人回合使用鎖定
            var skill1 = new HeroSkill
            {
                name = "敏捷",
                description = "你可以在他人回合使用鎖定。",
                autoActivate = false,
                checker = () =>
                {
                    if (playerStatus.GetHandCardCount(Lock) == 0)
                        return false;

                    if (judgement.currentPhase != NetworkJudgement.Phase.ChooseToSend)
                        return false;

                    if (judgement.currentRoundPlayerIndex == playerStatus.playerIndex)
                        return false;

                    return true;
                }
            };
            skill1.localActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 顯示手牌中的鎖定
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        netCanvas.ShowPlayerHandCard(
                            playerStatus.playerIndex,
                            card => _.data.suffix = card,
                            Lock
                        );
                        return Task.CompletedTask;
                    },
                    checker = (_) => {
                        if(_.data.suffix == int.MinValue)
                            return SkillAction.CheckerState.None;

                        return SkillAction.CheckerState.Continue;
                    }
                },
                // 選擇目標玩家
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
            skill1.serverActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 新增鎖定動作
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
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
                },
            };

            // 聯動 (WIP)
            // 當你使用鎖定時，抽三張牌，並分一張手牌給另一位玩家
            var skill2 = new HeroSkill
            {
                name = "聯動",
                description = "當你使用鎖定時，抽三張牌，並分一張手牌給另一位玩家。",
                autoActivate = true,
                checker = () =>
                {
                    if (manager.Judgement.currentPhase != NetworkJudgement.Phase.Reacting)
                        return false;

                    var cardAction = manager.Judgement.currentCardAction;
                    if (cardAction.user != playerStatus.playerIndex)
                        return false;

                    if (!((Card.CardSetting)cardAction.cardId).Compare(Lock))
                        return false;

                    return true;
                }
            };
            skill2.serverActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 抽三
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_) => {
                        playerStatus.DrawCard(3);
                        await playerStatus.InitReturnHandCardMenu(
                            playerStatus.playerIndex);
                    },
                    checker = (_) => {
                        if(playerStatus.isWaitingData)
                            return SkillAction.CheckerState.None;

                        return SkillAction.CheckerState.Continue;
                    }
                },
                // 選擇玩家
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        // 取出玩家選擇的卡片
                        _.data.suffix = playerStatus.tempData;
                        playerStatus.ClearTempData();
                        // 尚未實作Server -> Local 選擇玩家的方法
                        // 暫時設定為OtherPlayer()[0]
                        _.data.target = manager.GetOtherPlayers()[0];
                        return Task.CompletedTask;
                    },
                    checker = (_) => {
                        if(_.data.target == int.MinValue)
                            return SkillAction.CheckerState.None;

                        return SkillAction.CheckerState.Continue;
                    }
                },
                // 執行分卡片的動作
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        playerStatus.CardHToH(
                            _.data.suffix,
                            _.data.target);
                        return Task.CompletedTask;
                    }
                }
            };

            skills = new HeroSkill[] { skill1, skill2 };
        }

        protected override void BindSpecialMission()
        {

        }
    }
}

