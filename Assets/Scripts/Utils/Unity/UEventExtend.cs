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
                e.RemoveListener(proxy.Invoke);
                action();
            };

            e.AddListener(proxy.Invoke);
            return e;
        }

        public static UnityEvent<T> AutoRemoveListener<T>(this UnityEvent<T> e, Action<T> action)
        {
            Action<T> proxy = (_) => { };
            proxy = (_) =>
            {
                e.RemoveListener(proxy.Invoke);
                action(_);
            };

            e.AddListener(proxy.Invoke);
            return e;
        }
    }
}