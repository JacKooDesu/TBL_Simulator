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
        public UnityEvent<CardEnum.Property> OnChangeSelectCard { get; } = new();
        bool hasSelectFlag = false;

        public void SetSelect(ISelectable selectable)
        {
            hasSelectFlag = true;
            currentSelect = selectable;
            OnChangeSelectCard.Invoke((selectable as ISelectable<CardEnum.Property>)?.data ?? 0);
        }

        void Awake()
        {
            Singleton = this;
            UIs.AddRange(GetComponentsInChildren<ISetupWith<IPlayerStandalone>>());
        }

        public void SetupUI(IPlayerStandalone standalone)
        {
            foreach (var ui in UIs)
                ui.Setup(standalone);
        }
    }
}
