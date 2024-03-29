using System.Collections.Generic;
using System.Linq;
using TBL.Game.Sys;

namespace TBL.Game.Hero
{
    using TBL.Game.Sys.Helper;
    using Property = CardEnum.Property;
    public class Shining_Snipe : HeroSkill
    {
        protected override int _Id => Hero_Shining.SNIPE_SKILL_ID;
        protected override HeroId Hero => HeroId.Shining;
        public override void Bind(Manager manager, Player player, int index)
        {
            manager.PhaseManager.AfterEnter.AddListener(
                _ => player.SkillStatus.Update(index, UsageCheck(manager)));
        }
        public override bool UsageCheck(Manager manager) =>
            manager.Players.List.Where(x => x.TableHasBlack()).Count() is not 0;

        public override void Execute(Manager manager, Player player)
        {
            var targets =
                Filter(manager)
                    .Select(x => x.ProfileStatus.Id)
                    .ToArray();

            if (targets.Length is 0)
                return;

            GameAction_SelectPlayer selectPlayer = new(player, targets);
            selectPlayer.AndThen<int>(id => SelectCards(player, manager, id));
            selectPlayer.AddToFlow();
        }

        void SelectCards(Player user, Manager manager, int targetId)
        {
            var target = manager.Players.QueryById(targetId);
            var cards = target.CardStatus.Table;
            GameAction_SelectCard selectCards = new(user, new(cards.Select(x => x.AsProperty()).ToArray(), 3, 1));
            selectCards.AndThen<Property[]>(cards => Resolve(user,target, manager, cards));
            selectCards.AddToFlow();
        }

        void Resolve(Player user, Player target, Manager manager, Property[] cards)
        {
            manager.AddResolve(
                Phase_Resolving.ResolveDetail.Skill(
                    detail => manager.DiscardTable(
                        detail.target,
                        cards.Select(x => (int)x).ToArray()),
                    Hero,
                    _Id,
                    user,
                    target));
        }

        IEnumerable<Player> Filter(Manager manager) =>
            manager.Players
                   .List
                   .Where(x => x.TableHasBlack());
    }
}