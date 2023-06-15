using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TBL.Utils
{
    public static class UEventExtend
    {
        public static UnityEvent ReBind(this UnityEvent e, Action action, bool autoRemove = false)
        {
            e.RemoveAllListeners();
            if (autoRemove)
                return e.AutoRemoveListener(action);
            e.AddListener(action.Invoke);
            return e;
        }

        public static UnityEvent<T> ReBind<T>(this UnityEvent<T> e, Action<T> action, bool autoRemove = false)
        {
            e.RemoveAllListeners();
            if (autoRemove)
                return e.AutoRemoveListener(action);
            e.AddListener(action.Invoke);
            return e;
        }

        public static UnityEvent AutoRemoveListener(this UnityEvent e, Action action)
        {
            Action proxy = () => { };
            proxy = () =>
            {
                action();
                e.RemoveListener(proxy.Invoke);
            };

            e.AddListener(proxy.Invoke);
            return e;
        }

        public static UnityEvent<T> AutoRemoveListener<T>(this UnityEvent<T> e, Action<T> action)
        {
            Action<T> proxy = (_) => { };
            proxy = (_) =>
            {
                action(_);
                e.RemoveListener(proxy.Invoke);
            };

            e.AddListener(proxy.Invoke);
            return e;
        }
    }
}