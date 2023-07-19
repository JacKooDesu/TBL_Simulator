using System;
using UnityEngine.Events;

namespace TBL.Game.Sys
{
    public sealed partial class GameState : IDisposable
    {
        #region PHASE
        public UnityEvent E_Phase_Passing { get; } = new();
        public UnityEvent E_Phase_Recive { get; } = new();
        #endregion
    }
}