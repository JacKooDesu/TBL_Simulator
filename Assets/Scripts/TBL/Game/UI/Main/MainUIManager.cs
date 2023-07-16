using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace TBL.Game.UI.Main
{
    using Sys;

    public class MainUIManager : MonoBehaviour
    {
        public static MainUIManager Singleton { get; private set; }

        [Header("Windows")]
        [SerializeField] PlayerListWindow playerListWindow;
        public PlayerListWindow PlayerListWindow => playerListWindow;
        [SerializeField] List<ISetupWith<IPlayerStandalone>> UIs = new();

        ISelectable currentSelect;
        public UnityEvent<int?> OnChangeSelectCard { get; } = new();
        public UnityEvent<ISelectable> OnChangeSelect { get; } = new();
        bool hasSelectFlag = false;

        [SerializeField] CommonUI commonUI;
        [SerializeField] TempMenuManager tempMenuManager;
        public TempMenuManager TempMenuManager => tempMenuManager;

        public void SetSelect(ISelectable selectable)
        {
            hasSelectFlag = true;
            currentSelect = selectable;
            OnChangeSelect.Invoke(selectable);
            OnChangeSelectCard.Invoke((selectable as ISelectable<int>)?.data ?? null);
        }
        public void UpdateSelect() => SetSelect(currentSelect);

        void Awake()
        {
            Singleton = this;
            UIs.AddRange(GetComponentsInChildren<ISetupWith<IPlayerStandalone>>());
        }

        public void SetupUI(IPlayerStandalone standalone)
        {
            SetSelect(null);
            foreach (var ui in UIs)
                ui.Setup(standalone);

            commonUI = commonUI ?? FindObjectOfType<CommonUI>();
            commonUI?.Setup(standalone);

            BindEvent();
        }

        void BindEvent()
        {
            GameState.Instance.OnPhaseChange.AddListener(_ => UpdateSelect());
        }
    }
}
