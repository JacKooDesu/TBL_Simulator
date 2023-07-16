using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace TBL.Game
{
    using Sys;
    using static TBL.Game.PhaseQuestStatus;

    public static partial class CardEnum
    {
        public static bool ClientCheck(this Function function)
        {
            var phase = GameState.Instance.CurrentPhase;
            var isRoundHost =
                GameState.Instance.CurrentRoundHost == IPlayerStandalone.MyPlayer.ProfileStatus.Id;
            var isAskingRecive =
                (GameState.Instance.CurrnetPassingPlayer ?? -1) == IPlayerStandalone.MyPlayer.ProfileStatus.Id;

            var executable = function.ToExecutable();

            return CommonCheck(function, phase, isRoundHost, isAskingRecive) &&
                   // FIXME 尚未實作所有卡牌功能!!
                   (executable == null ? false : executable.ClientCheck());
        }

        public static bool ServerCheck(
            this Function function,
            Player player,
            Manager manager,
            Func<Player, Manager, bool> checkFunc = null)
        {
            var phase = manager.PhaseManager.Current().Type();
            var isRoundHost =
                manager.CurrentPlayer.ProfileStatus.Id == player.ProfileStatus.Id;
            var isAskingRecive =
                phase == PhaseType.Passing ?
                player.PhaseQuestStatus.Quest.Contains(QuestType.AskRecieve) :
                false;

            var executable = function.ToExecutable();

            return CommonCheck(function, phase, isRoundHost, isAskingRecive) &&
                   (checkFunc?.Invoke(player, manager) ?? true) &&
                   // FIXME 尚未實作所有卡牌功能!!
                   (executable == null ? false : executable.ServerCheck());
        }

        static bool CommonCheck(Function function, PhaseType phase, bool isRoundHost, bool isAskingRecive) =>
            function switch
            {
                Function.Burn => phase is not PhaseType.None,

                Function.Guess or
                Function.Gameble or
                Function.Lock or
                Function.Test => isRoundHost && phase is PhaseType.Main,

                Function.Intercept => !isAskingRecive && phase is PhaseType.Passing,
                Function.Skip => phase is PhaseType.Main,
                Function.Return => isAskingRecive && phase is PhaseType.Passing,
                Function.Invalidate => phase is PhaseType.Resolving,

                _ => false
            };
    }
}