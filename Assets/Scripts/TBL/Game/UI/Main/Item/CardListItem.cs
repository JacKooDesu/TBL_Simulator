using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TBL.UI.GameScene
{
    using Core.UI;
    using TBL.Game;
    using CardEnum = TBL.Game.CardEnum;
    using Game.UI;
    using System;

    public class CardListItem : MonoBehaviour, ISelectable<int>
    {
        public Text nameTextUI;
        public int cardID;
        public bool isSelected = false;

        // public UnityEvent<int> SelectEvent { get; } = new();
        [SerializeField] Button button;

        public SelectableType Type => SelectableType.Card;
        public UnityEvent<int> OnSelectEvent { get; } = new();
        public int data => cardID;

        public void SetUI(int id)
        {
            var property = (CardEnum.Property)id;
            cardID = id;
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
            button?.onClick.AddListener(() => OnSelectEvent.Invoke(data));
        }
    }

}
