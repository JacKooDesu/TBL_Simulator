using System.Collections.Generic;
using System.Threading.Tasks;
using TBL.UI.LogSystem;

namespace TBL.Hero
{
    public class Kin : HeroBase
    {
        protected override void BindSkill()
        {
            var skill1 = new HeroSkill(
                "棄卒保帥",
                $"當你的卡牌被另一位玩家 {RichTextHelper.TextWithBold("識破")} 時，可以翻開此角色牌，抽六張牌，選擇兩張按照任意順序放回牌庫頂。",
                false,
                () =>
                {
                    playerStatus.CmdChangeHeroState(false);
                    playerStatus.CmdDrawCard(6);

                    List<int> cardList = new List<int>();
                    for (int i = 0; i < 6; ++i)
                    {
                        cardList.Add(playerStatus.netHandCards[playerStatus.netHandCards.Count - 1 - i]);
                    }

                    ChooseCardToDeck(cardList);
                },
                () =>
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
            );

            var skill2 = new HeroSkill(
                "釜底抽薪",
                "當你獲得黑情報時，可以在自己面前放置一張黑情報，並在另一位玩家面前放置一張任意情報，並蓋伏此角色。",
                false,
                () =>
                {
                    var netCanvas = FindObjectOfType<NetCanvas.GameScene>();

                    playerStatus.CmdChangeHeroState(true);

                    netCanvas.ShowPlayerHandCard(
                        playerStatus.playerIndex,
                        (card) =>
                        {
                            playerStatus.CmdCardHToT(card, playerStatus.playerIndex);

                            netCanvas.BindSelectPlayer(
                                manager.GetOtherPlayers(),
                                (target) =>
                                    netCanvas.ShowPlayerHandCard(
                                        playerStatus.playerIndex,
                                        (giveCard) => playerStatus.CmdCardHToT(giveCard, target)
                                    )
                            );
                        },
                        new List<Card.CardColor>() { Card.CardColor.Black });
                },
                () =>
                {
                    if (playerStatus.netHandCards.Count <= 1 || playerStatus.GetHandCardColorCount(Card.CardColor.Black) == 0)
                        return false;

                    if (judgement.currentPhase != NetworkJudgement.Phase.Sending)
                        return false;

                    var targetPlayer = manager.players[judgement.currentSendingPlayer];
                    if (((Card.CardSetting)judgement.currentRoundSendingCardId).CardColor == Card.CardColor.Black &&
                        judgement.currentSendingPlayer == playerStatus.playerIndex &&
                        playerStatus.acceptCard)
                        return true;

                    return false;
                }
            );

            // ult - 使用後，你本回合使用的所有卡牌會直接生效。
            // 尚未設計
            // 可能需要於Judgement腳本新增onRoundOver訂閱，在回合結束階段將 limited=true

            skills = new HeroSkill[]{
                skill1,skill2
            };
        }

        protected override void BindSpecialMission()
        {
            mission = new HeroMission(
                $"手牌湊齊三張 {RichTextGeneral.red} 卡牌與三張 {RichTextGeneral.blue} 卡牌。",
                () =>
                {
                    return playerStatus.GetHandCardColorCount(Card.CardColor.Red) >= 3 &&
                            playerStatus.GetHandCardColorCount(Card.CardColor.Blue) >= 3;
                }
            );
        }

        async void ChooseCardToDeck(List<int> cardList)
        {
            var netCanvas = FindObjectOfType<NetCanvas.GameScene>();

            int cardCount = 0;

            bool hasSelect = false;

            System.Action selectCard = () => netCanvas.ShowCardMenu(
                cardList,
                (id) =>
                {
                    playerStatus.CmdCardHToD(id);
                    cardList.Remove(id);
                }
            );

            selectCard.Invoke();

            while (cardCount < 2)
            {
                if (hasSelect)
                {
                    hasSelect = false;
                    selectCard.Invoke();
                }

                await Task.Yield();
            }

            playerStatus.CmdSetSkillCanActivate(0, false);
        }
    }
}

