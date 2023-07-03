using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

namespace TBL.Game.UI.Main
{
    using UI;
    using Sys;
    using Utils;
    using TBL.UI.GameScene;
    public class CardListWindow : Window, ISetupWith<IPlayerStandalone>
    {
        [SerializeField] CardListItem prefab;
        List<CardListItem> items = new();

        [SerializeField] Transform content;

        [Header("Visual Setting")]
        [SerializeField] Color onSelectColor = new Color(1, 1, .2f, 1);
        [SerializeField] GameObject selector;

        public void Setup(IPlayerStandalone res)
        {
            if (res != IPlayerStandalone.Me)
                return;

            IPlayerStandalone.Me.player.CardStatus.OnChanged.AddListener(UpdateList);
            UpdateList(res.player.CardStatus);

            BindSelector();
        }

        public void UpdateList(CardStatus cs)
        {
            selector.SetActive(false);

            var cards = cs.Hand;

            content.DestroyChildren();

            foreach (var id in cards)
            {
                // var tempCard = CardSetting.IdToCard(id);
                var ui = Instantiate(prefab, content);
                ui.SetUI(id);
                ui.OnSelectEvent.AddListener(c => Debug.Log(c));
                ui.OnSelectEvent.AddListener(c => MainUIManager.Singleton.SetSelect(ui));

                items.Add(ui);
            }
        }

        void BindSelector()
        {
            MainUIManager.Singleton.OnChangeSelect.ReBind(
                s =>
                {
                    if (items.TryGet(x => (x as ISelectable) == s, out var target))
                    {
                        selector.transform.position = target.transform.position;
                        selector.SetActive(true);
                    }
                    else
                    // {
                    //     selector.transform.SetParent(transform);
                        selector.SetActive(false);
                    // }
                }
            );
        }
    }
}
