using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBL.NetCanvas;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Mirror;


namespace TBL.NetCanvas
{
    using Card;

    public partial class GameScene : NetCanvasBinderBase
    {
        #region TEMP_MENU
        [Header("暫存選單")]
        [SerializeField] UI.GameScene.Menu tempMenu;

        [SerializeField] List<UI.GameScene.Menu> tempMenuList = new List<UI.GameScene.Menu>();

        public void ShowPlayerCard(int index, UnityAction<int> action, params int[] requests)
        {
            List<int> cardIds = new List<int>(manager.players[index].netCards);
            ShowCardMenu(cardIds, action, requests);
        }

        public void ShowPlayerHandCard(int index, UnityAction<int> action, params int[] requests)
        {
            List<int> cardIds = new List<int>(manager.players[index].netHandCards);
            ShowCardMenu(cardIds, action, requests);
        }

        public void ShowCardMenu(List<int> cardIds, UnityAction<int> action, params int[] requests)
        {
            var cardIdList = new List<int>();
            if (requests.Length != 0)
                cardIdList = cardIds.FindAll(id => CardAttributeHelper.Compare(id, requests));
            else
                cardIdList = cardIds;

            var options = new List<Option>();

            foreach (var id in cardIdList)
            {
                var card = CardSetting.IdToCard(id);
                var option = new Option
                {
                    str = card.CardName,
                    onSelect = () => action.Invoke(id),
                    type = OptionType.CARD
                };
            }

            InitMenu(options);
        }

        public void ShowColorMenu(UnityAction<int> action, List<CardColor> requestColor)
        {
            var options = new List<Option>();
            foreach (var cc in requestColor)
            {
                options.Add(new Option
                {
                    str = CardSetting.ColorToString(cc),
                    onSelect = () => action.Invoke(((int)cc)),
                    type = OptionType.COLOR
                });
            }

            InitMenu(options);
        }

        public UI.GameScene.Menu InitMenu(List<Option> options, int defaultIndex = -1)
        {
            // Init Ui
            var menu = Instantiate(tempMenu, transform);
            menu.Init(options, defaultIndex);
            menu.onCloseEvent += () => tempMenuList.Remove(menu);
            tempMenuList.Add(menu);
            return menu;
        }

        public UI.GameScene.Menu InitMenu(int defaultIndex = -1, params Option[] options)
        {
            return InitMenu(new List<Option>(options), defaultIndex);
        }

        public void RemoveAllTempMenu()
        {
            // Debug.LogWarning("Remove all because phase changed!");
            var tempList = new List<UI.GameScene.Menu>(tempMenuList);
            foreach (var menu in tempList)
                menu.Cancel();
        }
        #endregion
    }
}
