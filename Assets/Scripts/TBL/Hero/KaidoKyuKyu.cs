using System.Collections.Generic;
using System.Threading.Tasks;
namespace TBL.Hero
{
    using Judgement;
    using static Card.CardAttributeHelper;
    using GameActionData;
    public class KaidoKyuKyu : HeroBase
    {
        protected override void BindSkill()
        {
            // 陰謀
            // 當一位玩家獲得你傳出的黑情報時，你可以抽取任意一位玩家一張手牌。
            var skill1 = new HeroSkill
            {
                name = "陰謀",
                description = "當一位玩家獲得你傳出的黑情報時，你可以抽取任意一位玩家一張手牌。",
                autoActivate = true,
                checker = () =>
                {
                    if (judgement.currentPhase != NetworkJudgement.Phase.Sending)
                        return false;

                    var targetPlayer = manager.players[judgement.currentSendingPlayer];
                    if (((Card.CardSetting)judgement.currentRoundSendingCardId).CardColor == Card.CardColor.Black &&
                        judgement.currentRoundPlayerIndex == playerStatus.playerIndex)
                        return true;

                    return false;
                }
            };
            skill1.serverActions = new SkillAction<GameActionData.ClassifyStruct<GameActionData.SkillActionData>>[]{
                // 詢問是否抽取卡片
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_) => {
                        await playerStatus.InitReturnDataMenu(
                            "抽取卡片",
                            "取消"
                        );
                    },
                    checker = (_) => {
                        if(playerStatus.isWaitingData)
                            return SkillAction.CheckerState.None;

                        if(playerStatus.tempData == 0)
                            return SkillAction.CheckerState.Continue;
                        else
                            return SkillAction.CheckerState.Break;
                    }
                },
                // 選擇對象
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_) => {
                        // 尚未實作Server對Local選擇對象方法
                        // _.data.target = manager.GetOtherPlayers()[0];
                        await playerStatus.ReturnPlayer(
                            playerStatus.GetOtherPlayers((p) => p.netHandCards.Count != 0));
                    },
                    checker = (_) => {
                        if(playerStatus.isWaitingData)
                            return SkillAction.CheckerState.None;

                        return SkillAction.CheckerState.Continue;
                    }
                },
                // 抽取卡片
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_) => {
                        _.data.target = playerStatus.tempData;
                        var target = manager.players[_.data.target];
                        var tempCardMenu = new string[target.GetHandCardCount()];
                        for(int i=0 ; i<tempCardMenu.Length ; ++i)
                            tempCardMenu[i] = "卡片";

                        await playerStatus.InitReturnDataMenu(tempCardMenu);
                    },
                    checker = (_) => {
                        if(playerStatus.isWaitingData)
                            return SkillAction.CheckerState.None;

                        return SkillAction.CheckerState.Continue;
                    }
                },
                // 執行抽卡動作
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        var target = manager.players[_.data.target];
                        target.CardHToH(
                            target.netHandCards[playerStatus.tempData],
                            playerStatus.playerIndex
                        );
                        return Task.CompletedTask;
                    }
                }
            };
            skill1.commonServerBreaker = () =>
                judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting;

            // 陽謀
            // 當你獲得傳出的黑情報時，你可以檢視一位玩家的手牌，並抽取一張手牌。
            var skill2 = new HeroSkill
            {
                name = "陽謀",
                description = "當你獲得傳出的黑情報時，你可以檢視一位玩家的手牌，並抽取一張手牌。",
                autoActivate = true,
                checker = () =>
                  {
                      if (judgement.currentPhase != NetworkJudgement.Phase.Sending)
                          return false;

                      var targetPlayer = manager.players[judgement.currentSendingPlayer];
                      if (((Card.CardSetting)judgement.currentRoundSendingCardId).CardColor == Card.CardColor.Black &&
                          judgement.currentSendingPlayer == playerStatus.playerIndex &&
                          playerStatus.acceptCard)
                          return true;

                      return false;
                  }
            };
            skill2.serverActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 詢問是否發動檢視手牌
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_) => {
                        await playerStatus.InitReturnDataMenu(
                            "檢視玩家手牌",
                            "取消"
                        );
                    },
                    checker = (_) => {
                        if(playerStatus.isWaitingData)
                            return SkillAction.CheckerState.None;

                        if(playerStatus.tempData == 0)
                            return SkillAction.CheckerState.Continue;
                        else
                            return SkillAction.CheckerState.Break;
                    }
                },
                // 選擇對象
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_) => {
                        // 尚未實作Server對Local選擇對象方法
                        // _.data.target = manager.GetOtherPlayers()[0];
                        await playerStatus.ReturnPlayer(
                            playerStatus.GetOtherPlayers((p) => p.netHandCards.Count != 0));
                    },
                    checker = (_) => {
                        if(playerStatus.isWaitingData)
                            return SkillAction.CheckerState.None;

                        return SkillAction.CheckerState.Continue;
                    }
                },
                // 執行顯示手牌動作
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_) => {
                        _.data.target = playerStatus.tempData;
                        await playerStatus.InitReturnHandCardMenu(
                            _.data.target
                        );
                    },
                    checker = (_) => {
                        if(playerStatus.isWaitingData)
                            return SkillAction.CheckerState.None;
                        else
                            return SkillAction.CheckerState.Continue;
                    }
                },
                // 抽取卡片
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_) => {
                        var target = manager.players[_.data.target];
                        var tempCardMenu = new string[target.GetHandCardCount()];
                        for(int i=0 ; i<tempCardMenu.Length ; ++i)
                            tempCardMenu[i] = "卡片";

                        await playerStatus.InitReturnDataMenu(tempCardMenu);
                    },
                    checker = (_) => {
                        if(playerStatus.isWaitingData)
                            return SkillAction.CheckerState.None;

                        return SkillAction.CheckerState.Continue;
                    }
                },
                // 執行抽卡動作
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        var target = manager.players[_.data.target];
                        var rand = UnityEngine.Random.Range(0, target.netHandCards.Count);
                        target.CardHToH(
                            target.netHandCards[rand],
                            playerStatus.playerIndex
                        );
                        return Task.CompletedTask;
                    }
                }
            };
            skill2.commonServerBreaker = () =>
                judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting;

            // 詭計
            // 將自己面前一張非假情報收為手牌。
            var skill3 = new HeroSkill
            {
                name = "詭計",
                description = "將自己面前一張非假情報收為手牌。",
                autoActivate = false,
                checker = () =>
                {
                    if (skills[2].limited)
                        return false;

                    return playerStatus.GetCardCount(Red) != 0 || playerStatus.GetCardCount(Blue) != 0;
                }
            };
            skill3.localActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 選擇面前情報
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        netCanvas.ShowPlayerCard(
                            playerStatus.playerIndex,
                            (id) => _.data.suffix = id
                        );
                        return Task.CompletedTask;
                    },
                    checker = (_) => {
                        if(_.data.suffix==int.MinValue)
                            return SkillAction.CheckerState.None;
                        else
                            return SkillAction.CheckerState.Finish;
                    }
                }
            };
            skill3.serverActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 執行情報拿回手中
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        playerStatus.SetSkillLimited(2, true);
                        playerStatus.CardTToH(
                            playerStatus.playerIndex,
                            _.data.suffix
                        );
                        return Task.CompletedTask;
                    }
                }
            };
            skill3.commonServerBreaker = () =>
                judgement.currentPhase != NetworkJudgement.Phase.HeroSkillReacting;

            skills = new HeroSkill[]{
                skill1,skill2,skill3
            };
        }

        protected override void BindSpecialMission()
        {
            mission = new HeroMission(
                "獲得十張或以上手牌。",
                () =>
                {
                    return playerStatus.netHandCards.Count >= 10;
                }
            );
        }

        void DrawTargetCard(int index)
        {
            var manager = ((NetworkRoomManager.singleton) as NetworkRoomManager);
            var netCanvas = FindObjectOfType<NetCanvas.GameScene>();

            List<string> cardList = new List<string>();
            List<UnityEngine.Events.UnityAction> actions = new List<UnityEngine.Events.UnityAction>();
            var player = manager.players[index];
            for (int i = 0; i < player.netHandCards.Count; ++i)
            {
                int x = i;
                int card = player.netHandCards[i];
                cardList.Add("卡牌");
                actions.Add(() => player.CmdCardHToH(card, playerStatus.playerIndex));
            }

            //  netCanvas.tempMenu.InitCustomMenu(cardList, actions);
        }
    }

}
