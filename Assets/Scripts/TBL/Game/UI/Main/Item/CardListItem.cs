using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TBL.UI.GameScene
{
    using TBL.Game;
    using CardEnum = TBL.Game.CardEnum;
    public class CardListItem : MonoBehaviour
    {
        public Text nameTextUI;
        public int cardID;
        public bool isSelected = false;

        NetCanvas.GameScene netCanvas;

        public void SetUI(int id)
        {
            var property = (CardEnum.Property)id;
            var color = property.ConvertColor();
            var function = property.ConvertFunction();
            var type = property.ConvertType();
            nameTextUI.color = color switch
            {
                CardEnum.Color.Blue => Color.blue,
                CardEnum.Color.Red => Color.red,
                CardEnum.Color.Black => Color.gray,
                _ => throw new System.Exception()
            };

            nameTextUI.text = function.ToDescription();
        }

        public void SetUI(ObsleteCard.CardSetting setting)
        {
            cardID = setting.ID;
            nameTextUI.text = setting.CardName;
            nameTextUI.color = setting.Color;

            JacDev.Utils.EventBinder.Bind(
                GetComponent<EventTrigger>(),
                EventTriggerType.PointerEnter,
                (e) =>
                {
                    if (!isSelected)
                        GetComponent<JacDev.Utils.UISlicker.ColorSlicker>().Slick("hover");
                }
            );

            JacDev.Utils.EventBinder.Bind(
                GetComponent<EventTrigger>(),
                EventTriggerType.PointerExit,
                (e) =>
                {
                    if (!isSelected)
                        GetComponent<JacDev.Utils.UISlicker.ColorSlicker>().SlickBack();
                }
            );

            JacDev.Utils.EventBinder.Bind(
                GetComponent<EventTrigger>(),
                EventTriggerType.PointerClick,
                (e) =>
                {
                    if (netCanvas.selectCard != null)
                    {
                        netCanvas.selectCard.isSelected = false;
                        netCanvas.selectCard.GetComponent<JacDev.Utils.UISlicker.ColorSlicker>().SlickBack();
                    }


                    isSelected = true;
                    netCanvas.selectCard = this;
                    GetComponent<JacDev.Utils.UISlicker.ColorSlicker>().Slick("select");
                }
            );

            string tipContent = "";

            switch (((ObsleteCard.CardSetting)setting.ID).SendType)
            {
                case ObsleteCard.CardSendType.Direct:
                    tipContent += "直達密電\n";
                    break;
                case ObsleteCard.CardSendType.Secret:
                    tipContent += "密電\n";
                    break;
                case ObsleteCard.CardSendType.Public:
                    tipContent += "公開文本\n";
                    break;
            }


            if (setting.CardType == ObsleteCard.CardType.Test)
            {
                ObsleteCard.Test t = new ObsleteCard.Test();
                t.ID = setting.ID;
                tipContent += t.Tip;
            }
            else
                tipContent += ((NetworkRoomManager)NetworkRoomManager.singleton).DeckManager.Deck.GetCardPrototype(setting.ID).Tip;

            GetComponent<TipTrigger>().content = tipContent;
        }

        // Start is called before the first frame update
        void Start()
        {
            netCanvas = FindObjectOfType<NetCanvas.GameScene>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
