using System;
using UnityEngine;
namespace TBL.Game.Sys
{
    public interface IStandaloneManager
    {
        bool InitializeComplete { get; }
        IPlayerStandalone[] GetStandalones();

        static IStandaloneManager Singleton { get; protected set; }
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void ResetSingleton() => Singleton = null;
#endif
    }
}
