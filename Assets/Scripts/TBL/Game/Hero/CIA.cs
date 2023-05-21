using UnityEngine;
using System.Collections.Generic;

namespace TBL.Game.Hero
{
    public class CIA : HeroBase
    {
        protected override void BindSkill()
        {
            // var skill1 = new HeroSkill
            // {
            //     name = "隔牆有耳",
            //     description = $"當一位玩家被 {RichTextHelper.TextWithBold("試探")} 時，你可以抽一張牌，然後捨棄一張手牌或 {RichTextHelper.TextWithBold("燒毀")} 你的一張 {RichTextHelper.TextWithBold("非假情報")} 。",
            //     autoActivate = false,
            //     action = (_) =>
            //      {
            //          var manager = ((NetworkRoomManager.singleton) as NetworkRoomManager);
            //          var netCanvas = FindObjectOfType<NetCanvas.GameScene>();

            //          playerStatus.CmdDrawCard(1);

            //          List<string> options = new List<string>();
            //          List<UnityEngine.Events.UnityAction> actions = new List<UnityEngine.Events.UnityAction>();
            //          options.Add("捨棄一張手牌");
            //          actions.Add(() =>
            //              netCanvas.ShowPlayerHandCard(playerStatus.playerIndex, (handCardIndex) =>
            //              {
            //                  playerStatus.CmdCardHToG(handCardIndex);
            //              })
            //          );

            //          if (PlayerStatus.netCards.FindIndex((card) => manager.DeckManager.Deck.GetCardPrototype(card).CardColor == Card.CardColor.Blue) != -1 ||
            //              PlayerStatus.netCards.FindIndex((card) => manager.DeckManager.Deck.GetCardPrototype(card).CardColor == Card.CardColor.Red) != -1)
            //          {
            //              options.Add("燒毀一張非假情報");
            //              actions.Add(() =>
            //                  netCanvas.ShowPlayerCard(playerStatus.playerIndex, (cardIndex) =>
            //                  {
            //                      playerStatus.CmdCardHToG(playerStatus.netCards[cardIndex]);
            //                  },
            //                  new List<Card.CardColor>() { Card.CardColor.Red, Card.CardColor.Blue })
            //              );
            //          }
            //         // netCanvas.InitMenu(options, actions);
            //      },
            //     checker = () =>
            //       {
            //           var netCanvas = FindObjectOfType<NetCanvas.GameScene>();

            //           if (manager.Judgement.currentPhase != NetworkJudgement.Phase.Reacting)
            //               return false;

            //           return ((Card.CardSetting)manager.Judgement.currentCardAction.cardId).CardType == Card.CardType.Test;
            //       }
            // };


            // skills = new HeroSkill[] {
            //     skill1
            // };
        }

        protected override void BindSpecialMission()
        {
            // this.mission = new HeroMission(
            //     $"獲得三張或以上的 {RichTextHelper.TextWithStyles("紅色情報", new RichTextHelper.SettingBase(RichTextHelper.Style.Bold), new RichTextHelper.Setting<Color>(RichTextHelper.Style.Color, Color.red))} 。",
            //     () =>
            //     {
            //         return playerStatus.GetCardCount(Card.CardColor.Red) >= 3;
            //     });
        }
    }
}

