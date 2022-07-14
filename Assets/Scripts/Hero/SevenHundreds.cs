using System.Threading.Tasks;

namespace TBL.Hero
{
    using GameAction;
    public class SevenHundreds : HeroBase
    {
        protected override void BindSkill()
        {
            // // 敏捷
            // // 可以在他人回合使用鎖定
            // var skill1 = new HeroSkill
            // {
            //     name = "敏捷",
            //     description = "你可以在他人回合使用鎖定。",
            //     autoActivate = false,
            //     localAction = async () =>
            //     {
            //         var skillAction = new SkillAction(user: playerStatus.playerIndex, skill: 0);
            //         netCanvas.ShowPlayerHandCard(
            //             playerStatus.playerIndex,
            //             (hCard) => skillAction.suffix = hCard);
            //         while (skillAction.suffix == int.MinValue)
            //             await Task.Yield();

            //         netCanvas.BindSelectPlayer(
            //             manager.GetOtherPlayers(),
            //             (index) => skillAction.target = index
            //         );
            //         while (skillAction.target == int.MinValue)
            //             await Task.Yield();

            //         return skillAction;
            //     },
            //     action = (_) =>
            //     {
            //         playerStatus.CmdTestCardAction(
            //             new CardAction(
            //                 _.user,
            //                 _.target,
            //                 _.suffix,
            //                 (int)Card.CardType.Lock,
            //                 0
            //             )
            //         );
            //     },
            //     checker = () =>
            //     {
            //         if (judgement.currentPhase != NetworkJudgement.Phase.ChooseToSend)
            //             return false;
            //         return judgement.currentPlayerIndex != playerStatus.playerIndex;
            //     }
            // };

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

            // skills = new HeroSkill[] {
            //     skill1,
            //     skill2
            // };
        }

        protected override void BindSpecialMission()
        {

        }
    }
}

