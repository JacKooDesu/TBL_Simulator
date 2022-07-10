using System;
using UnityEngine;
using System.Collections.Generic;

namespace TBL.UI
{
    public class TempMenuBase : MonoBehaviour, IWindowable
    {
        int defaultIndex;
        List<Option> options = new List<Option>();

        public virtual void Init(List<Option> options, int defaultIndex = -1)
        {
            this.options = options;
            if (defaultIndex == -1)
                this.defaultIndex = options.Count;
            else
                this.defaultIndex = defaultIndex;

            options.Add(new Option
            {
                str = "取消",
                onSelect = Close
            });
        }

        public virtual void Cancel()
        {
            options[defaultIndex].onSelect.Invoke();
        }

        public virtual void Close()
        {
            Destroy(gameObject);
        }
    }
}
