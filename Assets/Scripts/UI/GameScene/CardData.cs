using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TBL.UI.GameScene
{
    public class CardData : MonoBehaviour
    {
        public Text nameTextUI;
        public ushort cardID;
        public bool isSelected = false;

        NetCanvas.GameScene netCanvas;

        public void SetUI(Card.CardSetting setting)
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
