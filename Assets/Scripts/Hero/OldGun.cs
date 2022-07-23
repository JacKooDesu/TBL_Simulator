using System.Collections.Generic;
using System.Threading.Tasks;
using TBL.UI.LogSystem;
namespace TBL.Hero
{
    using GameAction;
    public class OldGun : HeroBase
    {
        protected override void BindSkill()
        {
            // // 就計
            // // 當你被試探或鎖定時，可以翻開此角色，然後抽兩張牌。
            // var skill1 = new HeroSkill
            // {
            //     name = "就計",
            //     description = "當你被試探或鎖定時，可以翻開此角色，然後抽兩張牌。",
            //     autoActivate = false,
            //     localAction = async () =>
            //     {
            //         await Task.Yield();
            //         return new SkillAction(skill: 0);
            //     },
            //     action = (_) =>
            //     {
            //         playerStatus.CmdDrawCard(2);
            //         playerStatus.CmdChangeHeroState(false);
            //     },
            //     checker = () =>
            //     {
            //         var netCanvas = FindObjectOfType<NetCanvas.GameScene>();
            //         var currentAction = manager.Judgement.currentCardAction;

            //         if (manager.Judgement.currentPhase != NetworkJudgement.Phase.Reacting)
            //             return false;

            //         return (((Card.CardSetting)currentAction.cardId).CardType == Card.CardType.Lock ||
            //                 ((Card.CardSetting)currentAction.cardId).CardType == Card.CardType.Test) &&
            //                 currentAction.target == playerStatus.playerIndex;
            //     }
            // };

            // // 合謀
            // // 蓋伏你的角色牌並和另一位角色互看身分牌，如果對方是未蓋伏的潛伏角色，你可以選擇蓋伏對方的角色牌並放置一張黑情報在面前
            // var skill2 = new HeroSkill
            // {
            //     name = "合謀",
            //     description = "蓋伏你的角色牌並和另一位角色互看身分牌，如果對方是未蓋伏的潛伏角色，你可以選擇蓋伏對方的角色牌並放置一張黑情報在面前。",
            //     autoActivate = false,
            //     localAction = async () =>
            //     {
            //         var skillAction = new SkillAction(user: playerStatus.playerIndex);
            //         netCanvas.BindSelectPlayer(
            //             manager.GetOtherPlayers(),
            //             (targetIndex) => skillAction.target = targetIndex
            //         );

            //         while (skillAction.target == int.MinValue)
            //             await Task.Yield();

            //         var targetHero = manager.players[skillAction.target].hero;
            //         if (targetHero.HeroType != HeroType.Hidden || targetHero.isHiding)
            //             return skillAction;

            //         var options = new List<Option>{
            //             new Option{
            //                 str = "蓋伏對方角色，並放置一張黑情報在面前",
            //                 onSelect=()=>skillAction.suffix = 1
            //             },
            //         };
            //         var menu = netCanvas.InitMenu(options);

            //         while (skillAction.target == int.MinValue && menu != null)
            //             await Task.Yield();

            //         return skillAction;
            //     },
            //     action = (_) =>
            //     {
            //         var target = manager.players[_.target];
            //         manager.RpcLog(
            //             new LogBase(
            //                 $"玩家 {playerStatus.playerIndex} ({playerStatus.playerName}) 為 {playerStatus.Team.name} 陣營",
            //                 true,
            //                 true,
            //                 _.target),
            //             target
            //         );

            //         manager.RpcLog(
            //             new LogBase(
            //                 $"玩家 {target.playerIndex} ({target.playerName}) 為 {target.Team.name} 陣營",
            //                 true,
            //                 true,
            //                 new int[] { playerStatus.playerIndex }),
            //             playerStatus
            //         );
            //     },
            //     checker = () =>
            //     {
            //         var judgement = ((NetworkRoomManager.singleton) as NetworkRoomManager).Judgement;
            //         return !isHiding && judgement.currentPhase != NetworkJudgement.Phase.Draw;
            //     }
            // };

            // var skill3 = new HeroSkill
            // {
            //     name = "攤牌",
            //     description = "翻開自己的身份牌，同時翻開另一位玩家的身份牌。",
            //     autoActivate = false,
            //     localAction = async () =>
            //     {
            //         var skillAction = new SkillAction(user: playerStatus.playerIndex, skill: 2);

            //         netCanvas.BindSelectPlayer(manager.GetOtherPlayers(), (index) => skillAction.target = index);

            //         while (skillAction.target == int.MinValue && !MouseInputHandler.rightClickInvoke)
            //             await Task.Yield();

            //         return skillAction;
            //     },
            //     action = (_) =>
            //     {
            //         var netCanvas = FindObjectOfType<NetCanvas.GameScene>();

            //         netCanvas.BindSelectPlayer(
            //             manager.GetOtherPlayers(),
            //             (targetIndex) =>
            //             {
            //                 var target = manager.players[targetIndex];

            //                 manager.RpcLog(
            //                     new LogBase(
            //                         $"玩家 {playerStatus.playerIndex} ({playerStatus.playerName}) 為 {playerStatus.Team.name} 陣營",
            //                         true,
            //                         false,
            //                         new int[] { }),
            //                     target
            //                 );

            //                 manager.RpcLog(
            //                     new LogBase(
            //                         $"玩家 {target.playerIndex} ({target.playerName}) 為 {target.Team.name} 陣營",
            //                         true,
            //                         false,
            //                         new int[] { }),
            //                     playerStatus
            //                 );
            //             });
            //     },
            //     checker = () =>
            //     {
            //         return true;
            //         var judgement = ((NetworkRoomManager.singleton) as NetworkRoomManager).Judgement;
            //         return !isHiding && judgement.currentPhase != NetworkJudgement.Phase.Draw;
            //     }
            // };

            // skills = new HeroSkill[]{
            //     skill1,skill2,skill3
            // };
        }

        protected override void BindSpecialMission()
        {
            mission = new HeroMission(
                $"獲得三張或以上 {RichTextGeneral.red} 情報",
                () =>
                {
                    return playerStatus.GetCardCount(Card.CardColor.Red) >= 3;
                }
            );
        }
    }
}

