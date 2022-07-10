using System.Collections.Generic;

namespace TBL
{
    public class Killer : Hero
    {
        protected override void BindSkill()
        {
            var skill1 = new HeroSkill(
                "連擊",
                "當一位玩家獲得你傳出的黑情報時，抽兩張牌，並可以在他面前再放置一張黑情報。",
                true,
                () =>
                {
                    playerStatus.CmdDrawCard(2);

                    if (playerStatus.GetHandCardColorCount(Card.CardColor.Black) > 0)
                    {
                        var netCanvas = FindObjectOfType<NetCanvas.GameScene>();
                        var manager = ((NetworkRoomManager.singleton) as NetworkRoomManager);

                        List<string> options = new List<string>();
                        List<UnityEngine.Events.UnityAction> actions = new List<UnityEngine.Events.UnityAction>();
                        options.Add("放置一張黑情報");
                        actions.Add(() =>
                            netCanvas.ShowPlayerHandCard(playerStatus.playerIndex, (handCardIndex) =>
                            {
                                playerStatus.CmdCardHToT(handCardIndex, manager.Judgement.currentSendingPlayer);
                            }, new List<Card.CardColor>() { Card.CardColor.Black })
                        );

                        options.Add("取消");
                        actions.Add(() =>
                        {
                            return;
                        });

                        netCanvas.tempMenu.InitCustomMenu(options, actions);
                    }
                },
                () =>
                {
                    var manager = ((NetworkRoomManager.singleton) as NetworkRoomManager);
                    var judgment = ((NetworkRoomManager.singleton) as NetworkRoomManager).Judgement;

                    if (judgment.currentPhase != NetworkJudgement.Phase.Sending)
                        return false;

                    var targetPlayer = manager.players[judgment.currentSendingPlayer];
                    if (((Card.CardSetting)judgment.currentRoundSendingCardId).CardColor == Card.CardColor.Black &&
                        judgment.currentRoundPlayerIndex == playerStatus.playerIndex)
                        return true;

                    return false;
                }
            );

            var skill2 = new HeroSkill(
                "預謀",
                $"你的 {RichTextHelper.TextWithBold("密電")} 可以當作 {RichTextHelper.TextWithBold("鎖定")} 使用。",
                false,
                () =>
                {
                    var netCanvas = FindObjectOfType<NetCanvas.GameScene>();
                    var manager = ((NetworkRoomManager.singleton) as NetworkRoomManager);

                    netCanvas.ShowPlayerHandCard(
                        playerStatus.playerIndex,
                        (card) =>
                        {
                            playerStatus.CmdCardHToG(card);
                            netCanvas.BindSelectPlayer(
                                manager.GetOtherPlayers(),
                                (index) =>
                                {
                                    playerStatus.CmdTestCardAction(new Action.CardAction(
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
                },
                () =>
                {
                    var judgment = ((NetworkRoomManager.singleton) as NetworkRoomManager).Judgement;

                    if (playerStatus.GetHandCardSendTypeCount(Card.CardSendType.Secret) == 0)
                        return false;

                    return (judgment.currentRoundPlayerIndex == playerStatus.playerIndex &&
                            judgment.currentPhase == NetworkJudgement.Phase.ChooseToSend);
                }
            );

            var skill3 = new HeroSkill(
                "冷血",
                $"可將任意手牌當作 {RichTextHelper.TextWithBold("鎖定")} 使用，且無法被識破。",
                false,
                () =>
                {
                    var manager = ((NetworkRoomManager.singleton) as NetworkRoomManager);
                    var netCanvas = FindObjectOfType<NetCanvas.GameScene>();

                    netCanvas.ShowPlayerHandCard(
                        playerStatus.playerIndex,
                        (card) =>
                        {
                            playerStatus.CmdCardHToG(card);
                            netCanvas.BindSelectPlayer(
                                manager.GetOtherPlayers(),
                                (index) => manager.players[index].isLocked = true
                            );
                        });
                },
                () =>
                {
                    var judgment = ((NetworkRoomManager.singleton) as NetworkRoomManager).Judgement;
                    return judgment.currentRoundPlayerIndex == playerStatus.playerIndex &&
                            judgment.currentPhase == NetworkJudgement.Phase.ChooseToSend;
                }
            );

            skills = new HeroSkill[]{
                skill1,skill2,skill3
            };
        }

        protected override void BindSpecialMission()
        {
            mission = new HeroMission(
                "親手讓另一位玩家成為第二位或以後死亡的玩家。",
                () =>
                {
                    var manager = ((NetworkRoomManager.singleton) as NetworkRoomManager);
                    var judgment = ((NetworkRoomManager.singleton) as NetworkRoomManager).Judgement;

                    if (judgment.currentPhase != NetworkJudgement.Phase.Sending)
                        return false;

                    var targetPlayer = manager.players[judgment.currentSendingPlayer];
                    if (targetPlayer.isDead && judgment.currentRoundPlayerIndex == playerStatus.playerIndex && manager.GetDeadPlayerCount() >= 2)
                        return true;

                    return false;
                }
            );
        }
    }
}

