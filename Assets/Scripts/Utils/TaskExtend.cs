using System.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;
namespace TBL.Util
{
    public static class TaskExtend
    {
        public static async Task WaitUntil(params Func<bool>[] conditions)
        {
            await Task.Run(
                async () =>
                {
                    Debug.LogWarning("Waiting!");
                    while (true)
                    {
                        foreach (var c in conditions)
                            if (c()) return;
                        await Task.Yield();
                    }
                }
            );
        }
    }
}