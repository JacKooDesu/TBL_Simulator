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
                        foreach (var c in conditions)
                            if (!c()) break;
                        await Task.Yield();
                    }
                }
            );
        }
    }
}