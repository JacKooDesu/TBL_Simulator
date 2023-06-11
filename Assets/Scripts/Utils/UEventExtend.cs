using System;
using UnityEngine;
using UnityEngine.Events;

namespace TBL.Utils
{
    public static class UEventExtend
    {
        public static UnityEvent ReBind(this UnityEvent e, Action action)
        {
            e.RemoveAllListeners();
            e.AddListener(action.Invoke);
            return e;
        }

        public static UnityEvent<T> ReBind<T>(this UnityEvent<T> e, Action<T> action)
        {
            e.RemoveAllListeners();
            e.AddListener(action.Invoke);
            return e;
        }
    }
}