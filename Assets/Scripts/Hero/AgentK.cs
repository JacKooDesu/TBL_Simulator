namespace TBL.Hero
{
    public class AgentK : HeroBase
    {
        protected override void BindSkill()
        {
            var skill1 = new HeroSkill(
                "警覺",
                $"玩家使用卡牌時，可以翻開此角色牌， {RichTextHelper.TextWithBold("識破") } 目標卡牌。(此技能 {RichTextHelper.TextWithBold("無法被識破") } )",
                false,
                () =>
                {
                    var manager = ((NetworkRoomManager.singleton) as NetworkRoomManager);

                    playerStatus.CmdChangeHeroState(false);
                    playerStatus.CmdTestCardAction(new Action.CardAction(playerStatus.playerIndex, 0, (int)Card.CardType.Invalidate, 0, 1));
                },
                () =>
                {
                    if (!isHiding)
                        return false;

                    var manager = ((NetworkRoomManager.singleton) as NetworkRoomManager);
                    return manager.Judgement.currentPhase == NetworkJudgement.Phase.Reacting &&
                            manager.Judgement.currentCardAction.cardId == 0;
                }
            );

            var skill2 = new HeroSkill(
                "掩護",
                "你獲得情報時，覆蓋此角色牌。",
                true,
                () =>
                {
                    var manager = ((NetworkRoomManager.singleton) as NetworkRoomManager);

                    playerStatus.CmdChangeHeroState(true);
                },
                () =>
                {
                    if (isHiding)
                        return false;

                    var judgement = ((NetworkRoomManager.singleton) as NetworkRoomManager).Judgement;
                    return judgement.currentPhase == NetworkJudgement.Phase.Sending &&
                           judgement.currentSendingPlayer == playerStatus.playerIndex &&
                           playerStatus.acceptCard;
                }
            );

            var skill3 = new HeroSkill(
                "反擊",
                $"當一位玩家使用 { RichTextHelper.TextWithBold("識破") } 時，獲得該 { RichTextHelper.TextWithBold("識破") } 並取消該識破效果。",
                false,
                () =>
                {
                    var judgement = ((NetworkRoomManager.singleton) as NetworkRoomManager).Judgement;

                    playerStatus.CmdAddHandCard(judgement.cardActionQueue[judgement.cardActionQueue.Count - 1].originCardId);
                    playerStatus.CmdTestCardAction(new Action.CardAction(playerStatus.playerIndex, 0, (int)Card.CardType.Invalidate, 0, 1));
                },
                () =>
                {
                    if (isHiding)
                        return false;

                    if (playerStatus.hero.skills[2].limited)
                        return false;

                    var judgement = ((NetworkRoomManager.singleton) as NetworkRoomManager).Judgement;

                    if (judgement.currentPhase != NetworkJudgement.Phase.Reacting || judgement.cardActionQueue.Count == 0)
                        return false;

                    var lastCardAction = judgement.cardActionQueue[judgement.cardActionQueue.Count - 1];

                    if (((Card.CardSetting)lastCardAction.cardId).CardType == Card.CardType.Invalidate &&
                        lastCardAction.suffix != -1)
                        return true;

                    return false;
                }
            );


            skills = new HeroSkill[] {
                skill1,skill2,skill3
            };
        }

        protected override void BindSpecialMission()
        {

        }
    }
}

