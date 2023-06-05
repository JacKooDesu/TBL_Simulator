using UnityEngine;

namespace TBL.Game.UI.Main
{
    using UI;
    using Sys;
    using Utils;
    public class CardListWindow : Window, ISetupWith<IPlayerStandalone>
    {
        [SerializeField] TBL.UI.GameScene.CardListItem prefab;

        [SerializeField] Transform content;
        public void Setup(IPlayerStandalone res)
        {
            IPlayerStandalone.Me.player.CardStatus.OnChanged += UpdateList;
        }

        public void UpdateList(CardStatus cs)
        {
            var cards = cs.Hand;

            content.DestroyChildren();

            foreach (var id in cards)
            {
                // var tempCard = CardSetting.IdToCard(id);
                var ui = Instantiate(prefab, content);
                ui.SetUI(id);
            }
        }
    }
}
