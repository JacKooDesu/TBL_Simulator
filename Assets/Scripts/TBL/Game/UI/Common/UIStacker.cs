using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBL.Game.UI
{
    public sealed class UIStacker : MonoBehaviour
    {
        public enum Index : int
        {
            Main = 100,
            TempWindow = 1000
        }

        public static UIStacker Singleton { get; private set; }

        void Awake()
        {
            if (Singleton != null)
                Destroy(gameObject);

            Singleton = this;
        }

        Dictionary<int, UIRegister> uiRegisters = new();
        Stack<UIData> uiStack = new();
        UIData focusUI;

        public void Regist(
            int index, string name,
            Action<object> activate,
            Action<object> reactivate,
            Action<object> deactivate)
        {
            var register = new UIRegister
            {
                Name = name,
                Activate = activate,
                Reactivate = reactivate,
                Deactivate = deactivate
            };
            uiRegisters[index] = register;
        }

        public void UnRegist(int index)
        {
            if (uiRegisters.ContainsKey(index))
                uiRegisters.Remove(index);
        }

        public void Push()
        {

        }

        public void Pop()
        {

        }

        struct UIRegister
        {
            public String Name;     // for debug
            public Action<object> Activate;
            public Action<object> Reactivate;
            public Action<object> Deactivate;
        }
        struct UIData
        {
            public readonly int Index;
            public UIData(int index) => Index = index;
        }
    }
}