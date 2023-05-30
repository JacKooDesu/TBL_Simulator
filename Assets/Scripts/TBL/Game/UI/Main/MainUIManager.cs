using System.Collections.Generic;
using UnityEngine;

namespace TBL.Game.UI.Main
{
    using Sys;

    public class MainUIManager : MonoBehaviour
    {
        public static MainUIManager Singleton { get; private set; }
        [SerializeField] List<ISetupWith<IPlayerStandalone>> UIs = new();

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
