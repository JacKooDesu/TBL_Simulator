using System;
using UnityEngine;
using System.Collections.Generic;

namespace TBL.Game
{
    using Sys;
    using static CardFunctionUtil;
    internal sealed class TestFunction : ICardFunction
    {
        public bool ClientCheck() => true;

        public bool ServerCheck() => true;

        public void ExecuteAction(Player user, Manager manager, int cardId)
        {
            SelectPlayer(user, manager, cardId);
        }

        void SelectPlayer(Player user, Manager manager, int cardId)
        {
            Action<int> ResolveAction =
                id => manager.AddResolve(
                    () => GetActions(manager, id, cardId).AddToFlow());

            SelectPlayerAction(user, manager, p => !user.Equals(p))
                .AndThen<int>(ResolveAction)
                .AddToFlow();
        }

        GameAction_SelectAction GetActions(
            Manager manager, int targetId, int cardId)
        {
            var unique = cardId.GetUniqueId();
            var player = Manager.Instance.Players.QueryById(targetId);
            var flag = false;   // TODO Hero 要有 flag

            var preset = GetPreset(unique);
            var presetStr = GetPresetString(preset);

            Action DrawAction = () => manager.Draw(player, 1);
            Action TalkAction = () => Debug.Log("我是一個好人");

            var mainAction = preset.isDraw ? DrawAction : TalkAction;
            var subAction = !preset.isDraw ? DrawAction : TalkAction;

            GameAction_SelectAction.Option mainOption = new(presetStr.main, mainAction);
            GameAction_SelectAction.Option subOption = new(presetStr.sub, subAction);

            List<GameAction_SelectAction.Option> list = new(flag ? 2 : 1);
            if (flag)
            {
                list.Add(mainOption);
                list.Add(subOption);
            }
            else
            {
                list.Add(
                    preset.team == player.TeamStatus.Current() ?
                    mainOption : subOption);
            }

            return new GameAction_SelectAction(player, new(list.ToArray()));
        }

        public record PresetSetting(TeamEnum team, bool isDraw);
        public PresetSetting GetPreset(int uniqueId) =>
            new(
                team: (uniqueId % 3) switch
                {
                    0 => TeamEnum.Blue,
                    1 => TeamEnum.Red,
                    2 => TeamEnum.Green,
                    _ => TeamEnum.None
                },
                isDraw: (uniqueId % 2) is 1);

        public static (string main, string sub) GetPresetString(PresetSetting preset)
        {
            (string mainTeam, string subTeam) = preset.team switch
            {
                TeamEnum.Blue => ("藍", "紅 / 綠"),
                TeamEnum.Red => ("紅", "藍 / 綠"),
                TeamEnum.Green => ("綠", "紅 / 藍"),
                _ => ("!?", "!?"),
            };
            (string mainAction, string subAction) =
                preset.isDraw ?
                ("抽牌", "說：我是一個好人") :
                ("說：我是一個好人", "抽牌");

            return ($"{mainTeam} - {mainAction}", $"{subTeam} - {subAction}");
        }
    }
}