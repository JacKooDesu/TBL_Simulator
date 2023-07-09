using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TBL.Game.UI.Main
{
    public sealed class CardOptionItem : OptionItemBase<CardEnum.Property>
    {
        [SerializeField] TMP_Text textUI;
        [SerializeField] Button button;
        public override SelectableType Type => SelectableType.Card;
        public override UnityEvent<CardEnum.Property> OnSelect { get; protected set; } = new();

        public override OptionItemBase<CardEnum.Property> SetData(CardEnum.Property data)
        {
            base.SetData(data);
            SetUI();
            return this;
        }

        void SetUI()
        {
            var property = Data;
            var color = property.ConvertColor();
            var function = property.ConvertFunction();
            var type = property.ConvertType();
            textUI.color = color switch
            {
                CardEnum.Color.Blue => Color.blue,
                CardEnum.Color.Red => Color.red,
                CardEnum.Color.Black => Color.gray,
                _ => throw new System.Exception()
            };

            textUI.text = function.ToDescription();
        }
    }
}
