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
        public UI.GameScene.Menu tempMenu;

        public void ShowPlayerCard(int index, UnityAction<int> action, List<CardColor> requestColor = null, List<CardSendType> requestSendType = null)
        {
            List<int> cardIds = manager.players[index].netCards.FindAll((id) => true);
            ShowCardMenu(cardIds, action, requestColor, requestSendType);
        }

        public void ShowPlayerHandCard(int index, UnityAction<int> action, List<CardColor> requestColor = null, List<CardSendType> requestSendType = null)
        {
            List<int> cardIds = manager.players[index].netHandCards.FindAll((id) => true);
            ShowCardMenu(cardIds, action, requestColor, requestSendType);
        }

        public void ShowCardMenu(List<int> cardIds, UnityAction<int> action, List<CardColor> requestColor = null, List<CardSendType> requestSendType = null)
        {
            // Init varieables
            if (requestColor == null)
                requestColor = new List<CardColor> { CardColor.Black, CardColor.Red, CardColor.Blue };
            if (requestSendType == null)
                requestSendType = new List<CardSendType> { CardSendType.Direct, CardSendType.Secret, CardSendType.Public };

            var cardIdList = cardIds.FindAll(
                id =>
                {
                    var card = CardSetting.IdToCard(id);
                    return requestColor.Contains(card.CardColor) && requestSendType.Contains(card.SendType);
                }
            );

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

        void InitMenu(List<Option> options, int defaultIndex = -1)
        {
            // Init Ui
            var menu = Instantiate(tempMenu.gameObject).GetComponent<UI.TempMenuBase>();
            menu.Init(options, defaultIndex);
        }
        #endregion
    }
}
