using UnityEngine;
using UnityEngine.Events;

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
                ui.OnSelectEvent.AddListener(c => Debug.Log(c));
                ui.OnSelectEvent.AddListener(c => MainUIManager.Singleton.SetSelect(ui));
            }
        }
    }
}
