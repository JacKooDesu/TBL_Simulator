using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TBL.Game
{
    using TBL.Game.Sys;
    public static partial class CardEnum
    {
        static ReadOnlyDictionary<Function, ICardFunction> FunctionExecuteDict => new(
            new Dictionary<Function, ICardFunction>{
                {Function.Lock, null},
                {Function.Skip, null},
                {Function.Return, null},
                {Function.Intercept, null},
                {Function.Guess, null},
                {Function.Burn, new BurnFunction()},
                {Function.Invalidate, null},
                {Function.Gameble, null},
                {Function.Test, null}});

        public static Action<Player, Manager> ExecuteAction(this Function function) =>
            FunctionExecuteDict[function].ExecuteAction;
    }
}
