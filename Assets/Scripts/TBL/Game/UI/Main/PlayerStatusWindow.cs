using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TBL.Game.UI.Main
{
    using Sys;
    using Networking;
    using System.Linq;
    using Option = Sys.GameAction_SelectAction.OptionBase;
    using TBL.Game.Hero;
    using System.Collections.Generic;
    using TBL.Utils;

    public class PlayerStatusWindow : MonoBehaviour, ISetupWith<IPlayerStandalone>
    {
        [SerializeField] Image heroImage;
        [SerializeField] TMP_Text heroNameText;

        [SerializeField] Image teamImage;
        [SerializeField] TMP_Text teamNameText;

        [SerializeField] Button _skillButton;
        bool _selectingSkill;
        List<int> AvailableSkills { get; } = new();

        public void Setup(IPlayerStandalone res)
        {
            if (res != IPlayerStandalone.Me)
                return;

            BindEvent(res.player);

            UpdateHero(res.player.HeroStatus);
            UpdateTeam(res.player.TeamStatus.Current());
            _skillButton.interactable = UpdateSkill(res.player.SkillStatus, res.player);
        }

        void BindEvent(Player player)
        {
            player.HeroStatus.OnChanged.ReBind(UpdateHero);
            player.TeamStatus.OnChanged.ReBind(UpdateTeam);
            player.SkillStatus.OnChanged.ReBind(status =>
                _skillButton.interactable = UpdateSkill(status, player));

            _skillButton.onClick.ReBind(() => SelectSkill(player));
        }

        void UpdateHero(HeroStatus status)
        {
            var asset = AssetHandler.Instance.Hero.Get(status.HeroId);
            heroNameText?.SetText(asset.name);
            heroImage.sprite = asset.icon;
        }

        void UpdateTeam(TeamEnum status)
        {
            var asset = AssetHandler.Instance.Team.Get(status);
            teamNameText?.SetText(asset.name);
            teamImage.sprite = asset.icon;
        }

        bool UpdateSkill(SkillStatus status, Player player)
        {
            AvailableSkills.Clear();
            AvailableSkills.AddRange(
                status
                    .GetAvailables()
                    .Select(x =>
                        HeroList.GetHero(player.HeroStatus.HeroId)?.GetSkillByIndex(x))
                    .Where(x => x is not null)
                    .Select(x => x.Id));

            return AvailableSkills.Count is not 0;
        }

        void SelectSkill(Player player)
        {
            if (AvailableSkills.Count is 0)
                return;
            if (_selectingSkill)
                return;
            _selectingSkill = true;

            var handler = AssetHandler.Instance.Hero;
            var heroId = (HeroId)player.HeroStatus.HeroId;

            var options =
                AvailableSkills
                    .Select(x => new Option(handler.GetSkill(x).Name))
                    .ToArray();
            var menu =
                MainUIManager
                    .Singleton
                    .TempMenuManager
                    .ActionTempMenu
                    .Create(new(options));

            Action<int> UseSkill = index =>
                IPlayerStandalone.Me
                    .Send(SendType.Cmd, new UseSkillPacket(heroId, AvailableSkills[index]));
            menu.OnConfirm.AutoRemoveListener(index =>
            {
                UseSkill(index);
                _selectingSkill = false;
                menu.Close();
            });
        }
    }
}