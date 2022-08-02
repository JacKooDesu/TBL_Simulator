﻿using System.Threading.Tasks;

namespace TBL.Hero
{
    using Util;
    using GameAction;
    using static Card.CardAttributeHelper;
    public class SevenHundreds : HeroBase
    {
        protected override void BindSkill()
        {
            // // 敏捷
            // 可以在他人回合使用鎖定
            var skill1 = new HeroSkill
            {
                name = "敏捷",
                description = "你可以在他人回合使用鎖定。",
                autoActivate = false,
                localAction = async (cancel) =>
                {
                    var sa = new SkillAction(user: playerStatus.playerIndex, skill: 0);
                    netCanvas.ShowPlayerHandCard(
                        playerStatus.playerIndex,
                        card => sa.suffix = card,
                        Lock
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
                        new CardAction
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
                    if (playerStatus.GetHandCardCount(Lock) == 0)
                        return false;

                    if (judgement.currentPhase != NetworkJudgement.Phase.ChooseToSend)
                        return false;

                    if (judgement.currentRoundPlayerIndex == playerStatus.playerIndex)
                        return false;

                    return true;
                }
            };

            // // 聯動 (WIP)
            // // 當你使用鎖定時，抽三張牌，並分一張手牌給另一位玩家
            // var skill2 = new HeroSkill
            // {
            //     name = "聯動",
            //     description = "當你使用鎖定時，抽三張牌，並分一張手牌給另一位玩家。",
            //     autoActivate = true,
            //     action = (_) =>
            //     {
            //         var netCanvas = FindObjectOfType<NetCanvas.GameScene>();
            //         playerStatus.CmdDrawCard(3);
            //         netCanvas.BindSelectPlayer(manager.GetOtherPlayers(), (target) =>
            //         {
            //             netCanvas.ShowPlayerHandCard(
            //                 playerStatus.playerIndex,
            //                 (id) =>
            //                 {
            //                     playerStatus.CmdCardHToH(id, target);
            //                 });
            //         });
            //     },
            //     checker = () =>
            //     {
            //         if (manager.Judgement.currentPhase != NetworkJudgement.Phase.Reacting)
            //             return false;

            //         var cardAction = manager.Judgement.currentCardAction;
            //         return
            //             cardAction.user == playerStatus.playerIndex &&
            //             manager.DeckManager.Deck.GetCardPrototype(cardAction.originCardId).CardType == Card.CardType.Lock;
            //     }
            // };

            skills = new HeroSkill[] { skill1 };
        }

        protected override void BindSpecialMission()
        {

        }
    }
}

