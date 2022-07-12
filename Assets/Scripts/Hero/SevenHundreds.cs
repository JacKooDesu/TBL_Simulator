namespace TBL.Hero
{
    public class SevenHundreds : HeroBase
    {
        protected override void BindSkill()
        {
            var skill1 = new HeroSkill("敏捷", "你可以在他人回合使用鎖定。", true);
            var skill2 = new HeroSkill(
                "聯動",
                "當你使用鎖定時，抽三張牌，並分一張手牌給另一位玩家。",
                false,
                () =>
                {
                    var manager = ((NetworkRoomManager.singleton) as NetworkRoomManager);
                    var netCanvas = FindObjectOfType<NetCanvas.GameScene>();
                    playerStatus.CmdDrawCard(3);
                    netCanvas.BindSelectPlayer(manager.GetOtherPlayers(), (target) =>
                    {
                        netCanvas.ShowPlayerHandCard(
                            playerStatus.playerIndex,
                            (id) =>
                            {
                                playerStatus.CmdCardHToH(id, target);
                            });
                    });
                },
                () =>
                {
                    var manager = ((NetworkRoomManager.singleton) as NetworkRoomManager);
                    if (manager.Judgement.currentPhase != NetworkJudgement.Phase.Reacting)
                        return false;

                    var cardAction = manager.Judgement.currentCardAction;
                    return
                        cardAction.user == playerStatus.playerIndex &&
                        manager.DeckManager.Deck.GetCardPrototype(cardAction.originCardId).CardType == Card.CardType.Lock;
                });

            skills = new HeroSkill[] {
                skill1,
                skill2
            };
        }

        protected override void BindSpecialMission()
        {
            
        }
    }
}

