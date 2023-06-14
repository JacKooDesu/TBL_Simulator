using System.Collections.Generic;
using UnityEngine;

namespace TBL.Game.UI.Main
{
    using UI;
    using Sys;
    public class PlayerListWindow : Window, ISetupWith<IPlayerStandalone>
    {
        [SerializeField] PlayerListItem prefab;
        [SerializeField] Transform content;
        [Header("顏色設定")]
        [SerializeField] Color roundHostColor;
        [SerializeField] Color cardPassingColor;

        Dictionary<IPlayerStandalone, PlayerListItem> playerItemDict = new();

        public void Setup(IPlayerStandalone res)
        {
            var item = Instantiate(prefab, content);
            item.Init(res);
            item.Bind();

            item.transform.SetSiblingIndex(res.Index);
            playerItemDict.Add(res, item);
        }
    }
}
