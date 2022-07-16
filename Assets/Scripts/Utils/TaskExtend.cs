using System.Threading.Tasks;
using System.Threading;
using System;

namespace TBL.Util
{
    public static class TaskExtend
    {
        public static async Task WaitUntil(params Func<bool>[] conditions)
        {
            await Task.Run(
                async () =>
                {
                    while (true)
                    {
                        UnityEngine.Debug.Log("Waiting");
                        foreach (var c in conditions)
                            if (c()) return;
                        await Task.Yield();
                    }
                }
            );
        }
    }
}