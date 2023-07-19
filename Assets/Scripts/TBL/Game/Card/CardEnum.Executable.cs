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
                {Function.Lock, new LockFunction()},
                {Function.Skip, new SkipFunction()},
                {Function.Return, new ReturnFunction()},
                {Function.Intercept, new InterceptFunction()},
                {Function.Guess, new GuessFunction()},
                {Function.Burn, new BurnFunction()},
                {Function.Invalidate, new InvalidateFunction()},
                {Function.Gameble, new GamebleFunction()},
                {Function.Test, new TestFunction()}});

        public static ICardFunction ToExecutable(this Function function) =>
            FunctionExecuteDict[function];
    }
}
