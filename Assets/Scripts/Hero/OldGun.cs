using System.Collections.Generic;
using TBL.UI.LogSystem;
namespace TBL.Hero
{
    public class OldGun : HeroBase
    {
        protected override void BindSkill()
        {
            var skill1 = new HeroSkill(
                "就計",
                "當你被試探或鎖定時，可以翻開此角色，然後抽兩張牌。",
                false,
                () =>
                {
                    playerStatus.CmdDrawCard(2);
                    playerStatus.CmdChangeHeroState(false);
                },
                () =>
                {
                    var netCanvas = FindObjectOfType<NetCanvas.GameScene>();
                    var currentAction = manager.Judgement.currentCardAction;

                    if (manager.Judgement.currentPhase != NetworkJudgement.Phase.Reacting)
                        return false;

                    return (((Card.CardSetting)currentAction.cardId).CardType == Card.CardType.Lock ||
                            ((Card.CardSetting)currentAction.cardId).CardType == Card.CardType.Test) &&
                            currentAction.target == playerStatus.playerIndex;
                }
            );

            var skill2 = new HeroSkill(
                "合謀",
                "蓋伏你的角色牌並和另一位角色互看身分牌，如果對方是未蓋伏的潛伏角色，你可以選擇蓋伏對方的角色牌並放置一張黑情報在面前。",
                false,
                () =>
                {
                    var netCanvas = FindObjectOfType<NetCanvas.GameScene>();

                    netCanvas.BindSelectPlayer(
                        manager.GetOtherPlayers(),
                        (targetIndex) =>
                        {
                            var target = manager.players[targetIndex];

                            manager.RpcLog(
                                new LogBase(
                                    $"玩家 {playerStatus.playerIndex} ({playerStatus.playerName}) 為 {playerStatus.Team.name} 陣營",
                                    true,
                                    true,
                                    new int[] { targetIndex }),
                                target
                            );

                            manager.RpcLog(
                                new LogBase(
                                    $"玩家 {target.playerIndex} ({target.playerName}) 為 {target.Team.name} 陣營",
                                    true,
                                    true,
                                    new int[] { playerStatus.playerIndex }),
                                playerStatus
                            );

                            if (target.hero.HeroType == HeroType.Hidden && !target.hero.isHiding)
                            {
                                netCanvas.tempMenu.InitCustomMenu(
                                    new List<string>(){
                                        "蓋伏對方角色，並放置一張黑情報在面前",
                                        "取消動作"
                                    },
                                    new List<UnityEngine.Events.UnityAction>(){
                                        ()=>
                                            netCanvas.ShowPlayerHandCard(
                                                playerStatus.playerIndex,
                                                (card)=>playerStatus.CmdCardHToT(card,playerStatus.playerIndex)
                                            )
                                        ,
                                        ()=>{}
                                    }
                                );
                            }
                        }
                    );
                },
                () =>
                {
                    var judgement = ((NetworkRoomManager.singleton) as NetworkRoomManager).Judgement;
                    return !isHiding && judgement.currentPhase != NetworkJudgement.Phase.Draw;
                }
            );

            var skill3 = new HeroSkill(
                "攤牌",
                "翻開自己的身份牌，同時翻開另一位玩家的身份牌。",
                false,
                () =>
                {
                    var netCanvas = FindObjectOfType<NetCanvas.GameScene>();

                    netCanvas.BindSelectPlayer(
                        manager.GetOtherPlayers(),
                        (targetIndex) =>
                        {
                            var target = manager.players[targetIndex];

                            manager.RpcLog(
                                new LogBase(
                                    $"玩家 {playerStatus.playerIndex} ({playerStatus.playerName}) 為 {playerStatus.Team.name} 陣營",
                                    true,
                                    false,
                                    new int[] { }),
                                target
                            );

                            manager.RpcLog(
                                new LogBase(
                                    $"玩家 {target.playerIndex} ({target.playerName}) 為 {target.Team.name} 陣營",
                                    true,
                                    false,
                                    new int[] { }),
                                playerStatus
                            );
                        });
                },
                () =>
                {
                    var judgement = ((NetworkRoomManager.singleton) as NetworkRoomManager).Judgement;
                    return !isHiding && judgement.currentPhase != NetworkJudgement.Phase.Draw;
                }
            );

            skills = new HeroSkill[]{
                skill1,skill2,skill3
            };
        }

        protected override void BindSpecialMission()
        {
            mission = new HeroMission(
                $"獲得三張或以上 {RichTextGeneral.red} 情報",
                () =>
                {
                    return playerStatus.GetCardColorCount(Card.CardColor.Red) >= 3;
                }
            );
        }
    }
}

