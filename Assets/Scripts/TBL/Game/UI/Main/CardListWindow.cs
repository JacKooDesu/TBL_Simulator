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
        public int SelectId { get; private set; }

        public void Setup(IPlayerStandalone res)
        {
            if (res != IPlayerStandalone.Me)
                return;

            IPlayerStandalone.Me.player.CardStatus.OnChanged += UpdateList;
            UpdateList(res.player.CardStatus);
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
                ui.SelectEvent.AddListener(id => SelectId = id);
                ui.SelectEvent.AddListener(id => Debug.Log(id));
            }
        }
    }
}
