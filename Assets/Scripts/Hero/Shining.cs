using System.Threading.Tasks;

namespace TBL.Hero
{
    public class Shining : HeroBase
    {
        protected override void BindSkill()
        {

            var skill1 = new HeroSkill(
                "狙擊",
                $"翻開此角色牌，然後燒毀 {RichTextHelper.TextWithBold("另一位玩家")} 面前的至多三張情報。",
                false,
                () =>
                {
                    var manager = ((NetworkRoomManager.singleton) as NetworkRoomManager);
                    var netCanvas = FindObjectOfType<NetCanvas.GameScene>();

                    int targetId = 0;

                    netCanvas.BindSelectPlayer(manager.GetOtherPlayers(), (i) =>
                    {
                        targetId = i;
                        var target = manager.players[targetId];
                        Snipe(target);
                    });


                },
                () =>
                {
                    if (playerStatus.hero.skills[0].limited)
                        return false;

                    var manager = ((NetworkRoomManager.singleton) as NetworkRoomManager);
                    bool haveCard = false;
                    foreach (var p in manager.players)
                    {
                        if (p.netCards.Count != 0)
                        {
                            haveCard = true;
                            break;
                        }
                    }
                    return playerStatus.hero.isHiding && haveCard;
                }
            );

            skills = new HeroSkill[] {
                skill1
            };
        }

        protected override void BindSpecialMission()
        {
            this.mission = new HeroMission(
                "親手讓另一位沒有獲得紅藍情報的玩家死亡。",
                () =>
                {
                    var manager = ((NetworkRoomManager.singleton) as NetworkRoomManager);
                    var judgment = ((NetworkRoomManager.singleton) as NetworkRoomManager).Judgement;

                    if (judgment.currentPhase != NetworkJudgement.Phase.Sending)
                        return false;

                    var targetPlayer = manager.players[judgment.currentSendingPlayer];
                    if (targetPlayer.isDead && judgment.currentRoundPlayerIndex == playerStatus.playerIndex)
                        return true;

                    return false;
                }
            );
        }

        async void Snipe(NetworkPlayer target)
        {
            var manager = ((NetworkRoomManager.singleton) as NetworkRoomManager);
            var netCanvas = FindObjectOfType<NetCanvas.GameScene>();

            int cardCount = 0;
            int burnCount = target.netCards.Count >= 3 ? 3 : target.netCards.Count;

            bool hasSelect = false;

            System.Action selectCard = () => netCanvas.ShowPlayerCard(target.playerIndex, (card) =>
            {
                playerStatus.CmdCardTToG(target.playerIndex, card);

                hasSelect = true;
                cardCount++;

                print(target.hero.HeroName);
            });

            selectCard.Invoke();

            while (cardCount < burnCount)
            {
                if (hasSelect)
                {
                    hasSelect = false;
                    selectCard.Invoke();
                }

                await Task.Yield();
            }

            playerStatus.CmdChangeHeroState(false);

            playerStatus.CmdSetSkillCanActivate(0, false);
            playerStatus.CmdSetSkillLimited(0, true);
        }
    }
}

