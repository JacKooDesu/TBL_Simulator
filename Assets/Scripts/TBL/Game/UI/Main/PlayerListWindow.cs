using UnityEngine;

namespace TBL.Game.UI.Main
{
    using UI;
    using Sys;
    public class PlayerListWindow : Window, ISetupWith<IPlayerStandalone>
    {
        [SerializeField] PlayerListItem prefab;

        [SerializeField] Transform content;
        public void Setup(IPlayerStandalone res)
        {
            var item = Instantiate(prefab, content);
            item.Init(res);

            item.transform.SetSiblingIndex(res.Index);
        }
    }
}
