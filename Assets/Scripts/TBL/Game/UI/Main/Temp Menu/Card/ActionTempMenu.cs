using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TBL.Game.UI.Main
{
    using TBL.Utils;
    using Sys;
    using ActionOption = Sys.GameAction_SelectAction.Option;
    public sealed class ActionTempMenu :
    TempMenuBase<ActionOption, ActionTempMenu.SetupData, int>
    {
        public record SetupData(ActionOption[] actions) : SetupDataBase(actions);

        [SerializeField] Button confirmBtn;
        [SerializeField] Button cancelBtn;

        int CurrentSelect { get; set; }

        public override TempMenuBase<ActionOption, ActionTempMenu.SetupData, int> Create(SetupData data)
        {
            base.Create(data);
            confirmBtn.onClick.ReBind(Confirm);
            OnSelectEvent.ReBind(Select);
            CurrentSelect = -1;
            Check();
            return this;
        }
        void Confirm() =>
            OnConfirm.Invoke(CurrentSelect);

        void Select(OptionItemBase<ActionOption> item)
        {
            CurrentSelect = Data.actions.ToList().IndexOf(item.Data);
            Check();
        }

        void Check() =>
            confirmBtn.interactable = CurrentSelect != -1;

        public override void Cancel() { }
    }
}
