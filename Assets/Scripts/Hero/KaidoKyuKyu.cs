using System.Collections.Generic;

namespace TBL.Hero
{
    public class KaidoKyuKyu : HeroBase
    {
        protected override void BindSkill()
        {
            var skill1 = new HeroSkill(
                "陰謀",
                "當一位玩家獲得你傳出的黑情報時，你可以抽取任意一位玩家一張手牌。",
                false,
                () =>
                {
                    var netCanvas = FindObjectOfType<NetCanvas.GameScene>();

                    List<int> playerList = new List<int>();
                    foreach (var p in manager.players)
                    {
                        if (p.netHandCards.Count > 0)
                            playerList.Add(p.playerIndex);
                    }

                    netCanvas.BindSelectPlayer(playerList, (index) => DrawTargetCard(index));
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
                "陽謀",
                "當你獲得傳出的黑情報時，你可以檢視一位玩家的手牌，並抽取一張手牌。",
                false,
                () =>
                {
                    var netCanvas = FindObjectOfType<NetCanvas.GameScene>();

                    List<int> playerList = new List<int>();
                    foreach (var p in manager.players)
                    {
                        if (p.netHandCards.Count > 0)
                            playerList.Add(p.playerIndex);
                    }

                    netCanvas.BindSelectPlayer(
                        playerList,
                        (index) => netCanvas.ShowPlayerHandCard(index, (c) => DrawTargetCard(index)));
                },
                () =>
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
            );

            var skill3 = new HeroSkill(
                "詭計",
                "將自己面前一張非假情報收為手牌。",
                false,
                () =>
                {
                    skills[2].limited = true;

                    var netCanvas = FindObjectOfType<NetCanvas.GameScene>();

                    netCanvas.ShowPlayerCard(
                       playerStatus.playerIndex,
                       (card) => playerStatus.CmdCardTToH(playerStatus.playerIndex, card),
                       new List<Card.CardColor>() { Card.CardColor.Red, Card.CardColor.Blue }
                    );
                },
                () =>
                {
                    if (skills[2].limited)
                        return false;

                    return playerStatus.GetCardColorCount(Card.CardColor.Red) != 0 || playerStatus.GetCardColorCount(Card.CardColor.Blue) != 0;
                }
            );

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

            netCanvas.tempMenu.InitCustomMenu(cardList, actions);
        }
    }

}
