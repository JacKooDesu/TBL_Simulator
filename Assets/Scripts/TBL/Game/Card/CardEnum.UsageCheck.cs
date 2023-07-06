using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace TBL.Game
{
    using Sys;
    public static partial class CardEnum
    {
        public static bool Check(this Function function)
        {
            var phase = GameState.Instance.CurrentPhase;
            var isRoundHost =
                GameState.Instance.CurrentRoundHost == IPlayerStandalone.MyPlayer.ProfileStatus.Id;
            var isAskingRecive =
                (GameState.Instance.CurrnetPassingPlayer ?? -1) == IPlayerStandalone.MyPlayer.ProfileStatus.Id;

            return function switch
            {
                Function.Burn => phase is not PhaseType.None,
                
                Function.Guess or
                Function.Gameble or
                Function.Lock or
                Function.Test => isRoundHost && phase is PhaseType.Main,

                Function.Intercept => !isAskingRecive && phase is PhaseType.Passing,
                Function.Skip => phase is PhaseType.Main,
                Function.Return => isAskingRecive && phase is PhaseType.Passing,
                Function.Invalidate => phase is PhaseType.Reacting,
                
                _ => false
            };
        }
    }
}