using System.Collections.Generic;
using System.Threading.Tasks;
using TBL.UI.LogSystem;

namespace TBL.Game.Hero
{
    using Judgement;
    using static Card.CardAttributeHelper;
    using GameActionData;
    public class Kin : HeroBase
    {
        protected override void BindSkill()
        {
            // 棄卒保帥
            // 當你的卡牌被另一位玩家識破時，可以翻開此角色牌，抽六張牌，選擇兩張按照任意順序放回牌庫頂。
            var skill1 = new HeroSkill
            {
                name = "棄卒保帥",
                description = $"當你的卡牌被另一位玩家 {RichTextHelper.TextWithBold("識破")} 時，可以翻開此角色牌，抽六張牌，選擇兩張按照任意順序放回牌庫頂。",
                autoActivate = false,
                checker = () =>
                {
                    var judgment = ((NetworkRoomManager.singleton) as NetworkRoomManager).Judgement;
                    if (judgment.currentPhase == NetworkJudgement.Phase.Reacting)
                    {
                        var actionIndex = judgment.cardActionQueue.IndexOf(judgment.currentCardAction);
                        if (actionIndex == -1 || actionIndex + 1 >= judgment.cardActionQueue.Count)
                            return false;

                        return ((Card.CardSetting)judgment.currentCardAction.cardId).CardType == Card.CardType.Invalidate &&
                                judgment.cardActionQueue[actionIndex + 1].user == playerStatus.playerIndex;
                    }
                    return false;
                }
            };
            skill1.serverActions = new SkillAction<GameActionData.ClassifyStruct<GameActionData.SkillActionData>>[]{
                // 抽六，選二
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_) => {
                        int[] cards;
                        if(_.data.suffix == int.MinValue){
                            cards = playerStatus.DrawCard(6);
                            _.tempObjects.Add(cards);   // index = 0，存放卡片
                            _.data.suffix = 0;  // 紀錄已選擇數量
                        }
                        else{
                            cards = _.tempObjects[0] as int[];
                        }

                        await playerStatus.InitReturnCustomCardMenu(cards);
                        _.data.suffix++;
                    },
                    checker = (_) => {
                        if(playerStatus.isWaitingData)
                            return SkillAction.CheckerState.None;

                        return SkillAction.CheckerState.Continue;
                    }
                },
                // 卡片放回牌頂動作
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        playerStatus.CardHToD(playerStatus.tempData);
                        return Task.CompletedTask;
                    },
                    checker = (_) => {
                        if(_.data.suffix != 2)
                            return SkillAction.CheckerState.Restart;
                        else
                            return SkillAction.CheckerState.Finish;
                    }
                }
            };

            // 釜底抽薪
            // 當你獲得黑情報時，可以在自己面前放置一張黑情報，並在另一位玩家面前放置一張任意情報，並蓋伏此角色。
            var skill2 = new HeroSkill
            {
                name = "釜底抽薪",
                description = "當你獲得黑情報時，可以在自己面前放置一張黑情報，並在另一位玩家面前放置一張任意情報，並蓋伏此角色。",
                autoActivate = true,

                checker = () =>
                {
                    if (playerStatus.netHandCards.Count <= 1 || playerStatus.GetHandCardCount(Black) == 0)
                        return false;

                    if (judgement.currentPhase != NetworkJudgement.Phase.Sending)
                        return false;

                    var targetPlayer = manager.players[judgement.currentSendingPlayer];
                    if (((Card.CardSetting)judgement.currentRoundSendingCardId).Compare(Black) &&
                        judgement.currentSendingPlayer == playerStatus.playerIndex &&
                        playerStatus.acceptCard)
                        return true;

                    return false;
                }
            };
            skill2.serverActions = new SkillAction<ClassifyStruct<SkillActionData>>[]{
                // 詢問是否發動
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_) => {
                        await playerStatus.InitReturnDataMenu("釜底抽薪","取消");
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
                // 選擇黑情報放置
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_) => {
                        await playerStatus.InitReturnHandCardMenu(playerStatus.playerIndex, Black);
                    },
                    checker = (_) => {
                        if(playerStatus.isWaitingData)
                            return SkillAction.CheckerState.None;

                        return SkillAction.CheckerState.Continue;
                    }
                },
                // 執行黑情報放置，選擇放置情報
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_) => {
                        playerStatus.AddCard(playerStatus.tempData);
                        await playerStatus.InitReturnHandCardMenu(playerStatus.playerIndex);
                    },
                    checker = (_) => {
                        if(playerStatus.isWaitingData)
                            return SkillAction.CheckerState.None;

                        return SkillAction.CheckerState.Continue;
                    }
                },
                // 選擇對象
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = async (_) => {
                        _.data.suffix = playerStatus.tempData;
                        await playerStatus.ReturnPlayer(playerStatus.GetOtherPlayers((p) => true));
                    },
                    checker = (_) => {
                        if(playerStatus.isWaitingData)
                            return SkillAction.CheckerState.None;

                        return SkillAction.CheckerState.Continue;
                    }
                },
                // 放置情報
                new SkillAction<ClassifyStruct<SkillActionData>>(){
                    action = (_) => {
                        manager.players[playerStatus.tempData].AddCard(_.data.suffix);
                        return Task.CompletedTask;
                    }
                }
            };


            // ult - 使用後，你本回合使用的所有卡牌會直接生效。
            // 尚未設計
            // 可能需要於Judgement腳本新增onRoundOver訂閱，在回合結束階段將 limited=true

            skills = new HeroSkill[]{
                skill1,skill2
            };
        }

        protected override void BindSpecialMission()
        {
            // mission = new HeroMission(
            //     $"手牌湊齊三張 {RichTextGeneral.red} 卡牌與三張 {RichTextGeneral.blue} 卡牌。",
            //     () =>
            //     {
            //         return playerStatus.GetHandCardCount(Card.CardColor.Red) >= 3 &&
            //                 playerStatus.GetHandCardCount(Card.CardColor.Blue) >= 3;
            //     }
            // );
        }

        // async void ChooseCardToDeck(List<int> cardList)
        // {
        //     var netCanvas = FindObjectOfType<NetCanvas.GameScene>();

        //     int cardCount = 0;

        //     bool hasSelect = false;

        //     System.Action selectCard = () => netCanvas.ShowCardMenu(
        //         cardList,
        //         (id) =>
        //         {
        //             playerStatus.CmdCardHToD(id);
        //             cardList.Remove(id);
        //         }
        //     );

        //     selectCard.Invoke();

        //     while (cardCount < 2)
        //     {
        //         if (hasSelect)
        //         {
        //             hasSelect = false;
        //             selectCard.Invoke();
        //         }

        //         await Task.Yield();
        //     }

        //     playerStatus.CmdSetSkillCanActivate(0, false);
        // }
    }
}

