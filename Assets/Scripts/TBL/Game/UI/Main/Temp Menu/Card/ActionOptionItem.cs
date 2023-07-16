using System.Collections;
using System.Collections.Generic;
using TBL.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TBL.Game.UI.Main
{
    using TBL.Utils;
    using Sys;
    using ActionOption = Sys.GameAction_SelectAction.Option;

    public sealed class ActionOptionItem : OptionItemBase<ActionOption>
    {
        [SerializeField] TMP_Text textUI;
        [SerializeField] Button button;
        public override SelectableType Type => SelectableType.Card;
        public override UnityEvent<ActionOption> OnSelect { get; protected set; } = new();

        public override OptionItemBase<ActionOption> SetData(ActionOption data)
        {
            base.SetData(data);
            SetUI();
            return this;
        }

        void SetUI()
        {
            textUI.text = Data.name;
            button.onClick.ReBind(() => OnSelect.Invoke(Data));
        }
    }
}
