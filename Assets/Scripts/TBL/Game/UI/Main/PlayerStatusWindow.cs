using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TBL.Game.UI.Main
{
    using Sys;
    using Networking;

    public class PlayerStatusWindow : MonoBehaviour, ISetupWith<IPlayerStandalone>
    {
        [SerializeField] Image heroImage;
        [SerializeField] TMP_Text heroNameText;

        [SerializeField] Image teamImage;
        [SerializeField] TMP_Text teamNameText;

        public void Setup(IPlayerStandalone res)
        {
            if (res != IPlayerStandalone.Me)
                return;

            BindEvent(res.player);

            UpdateHero(res.player.HeroStatus);
            UpdateTeam(res.player.TeamStatus.Current());
        }

        void BindEvent(Player player)
        {
            player.HeroStatus.OnChanged.AddListener(UpdateHero);
            player.TeamStatus.OnChanged.AddListener(UpdateTeam);
        }

        void UpdateHero(HeroStatus status)
        {
            var asset = AssetHandler.Instance.Hero.Get(status.HeroId);
            heroImage.sprite = asset.icon;
        }

        void UpdateTeam(TeamEnum status)
        {
            var asset = AssetHandler.Instance.Team.Get(status);
            teamImage.sprite = asset.icon;
        }
    }
}