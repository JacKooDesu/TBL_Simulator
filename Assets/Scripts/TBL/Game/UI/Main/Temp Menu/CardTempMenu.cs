using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TBL.Game.UI.Main
{
    using TBL.Utils;
    using Property = CardEnum.Property;
    public sealed class CardTempMenu :
    TempMenuBase<Property, CardTempMenu.SetupData, Property[]>
    {
        public record SetupData(Property[] cards, int count) : SetupDataBase(cards);

        private Button confirmBtn;
        private Button cancelBtn;

        List<OptionItemBase<Property>> SelectedCardList { get; } = new();

        public override TempMenuBase<Property, CardTempMenu.SetupData, Property[]> Create(SetupData data)
        {
            base.Create(data);
            SelectedCardList.Clear();
            confirmBtn.onClick.ReBind(Confirm);
            OnSelectEvent.ReBind(Swap);
            return this;
        }
        void Confirm() =>
            OnConfirm.Invoke(SelectedCardList.Select(x => x.Data).ToArray());

        void Swap(OptionItemBase<CardEnum.Property> item)
        {
            if (SelectedCardList.Contains(item))
                SelectedCardList.Remove(item);
            else
                SelectedCardList.Add(item);
        }

        public override void Cancel() { }
    }
}
