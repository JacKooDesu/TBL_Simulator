using System.Collections.Generic;

namespace TBL.Hero
{
    using Util;
    using GameAction;

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

                    // 沒有黑情報可選
                    if (playerStatus.GetCardCount(Card.CardColor.Black) <= 0)
                        return true;

                    var targetPlayer = manager.players[judgement.currentSendingPlayer];

                    playerStatus.ClearTempData();
                    await playerStatus.InitReturnHandCardMenu(Card.CardColor.Black);
                    await TaskExtend.WaitUntil(
                        () => !playerStatus.isWaitingData
                    );

                    targetPlayer.AddCard(playerStatus.tempData);

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

            // WIP
            var skill2 = new HeroSkill
            {
                name = "預謀",
                description = $"你的 {RichTextHelper.TextWithBold("密電")} 可以當作 {RichTextHelper.TextWithBold("鎖定")} 使用。",
                autoActivate = false,
                localAction = async (cancel) =>
                {
                    var sa = new SkillAction
                    {
                        skill = 1,
                        user = playerStatus.playerIndex
                    };

                    netCanvas.ShowPlayerHandCard(
                        index: playerStatus.playerIndex,
                        action: (id) =>
                        {
                            manager.DeckManager.Deck.GetCardPrototype((int)Card.CardType.Lock).OnUse(playerStatus, id);
                        }
                    );
                    await TaskExtend.WaitUntil(
                        () => sa.suffix != int.MinValue,
                        () => cancel.IsCancellationRequested);

                    return sa;
                },
                action = async (_) =>
                {
                    var netCanvas = FindObjectOfType<NetCanvas.GameScene>();

                    netCanvas.ShowPlayerHandCard(
                        playerStatus.playerIndex,
                        (card) =>
                        {
                            playerStatus.CmdCardHToG(card);
                            netCanvas.BindSelectPlayer(
                                manager.GetOtherPlayers(),
                                (index) =>
                                {
                                    playerStatus.CmdTestCardAction(new GameAction.CardAction(
                                        playerStatus.playerIndex,
                                        index,
                                        (int)Card.CardType.Lock,
                                        0,
                                        -1
                                    ));
                                }
                            );
                        },
                        null,
                        new List<Card.CardSendType>() { Card.CardSendType.Secret }
                    );

                    return true;
                },
                checker = () =>
                {
                    var judgment = ((NetworkRoomManager.singleton) as NetworkRoomManager).Judgement;

                    if (playerStatus.GetHandCardCount(Card.CardSendType.Secret) == 0)
                        return false;

                    if (judgment.currentRoundPlayerIndex != playerStatus.playerIndex)
                        return false;

                    if (judgment.currentPhase == NetworkJudgement.Phase.ChooseToSend)
                        return false;

                    return true;
                }
            };

            // var skill3 = new HeroSkill
            // {
            //     name = "冷血",
            //     description = $"可將任意手牌當作 {RichTextHelper.TextWithBold("鎖定")} 使用，且無法被識破。",
            //     autoActivate = false,
            //     action = (_) =>
            //       {
            //           var netCanvas = FindObjectOfType<NetCanvas.GameScene>();

            //           netCanvas.ShowPlayerHandCard(
            //               playerStatus.playerIndex,
            //               (card) =>
            //               {
            //                   playerStatus.CmdCardHToG(card);
            //                   netCanvas.BindSelectPlayer(
            //                       manager.GetOtherPlayers(),
            //                       (index) => manager.players[index].isLocked = true
            //                   );
            //               });
            //       },
            //     checker = () =>
            //       {
            //           return judgement.currentRoundPlayerIndex == playerStatus.playerIndex &&
            //                   judgement.currentPhase == NetworkJudgement.Phase.ChooseToSend;
            //       }
            // };

            skills = new HeroSkill[]{
                skill1
};
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

