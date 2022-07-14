using System;
using UnityEngine;
using System.Collections.Generic;

namespace TBL.UI
{
    // 只管理數值
    public class TempMenuBase : MonoBehaviour, IWindowable
    {
        protected int defaultIndex;
        protected List<Option> options = new List<Option>();

        public virtual void Init(List<Option> options, int defaultIndex = -1)
        {
            this.options = new List<Option>(options);
            this.defaultIndex = defaultIndex;
        }

        public virtual void Cancel()
        {
            if (defaultIndex == -1)
                Close();
            else
                options[defaultIndex].onSelect.Invoke();
        }

        public virtual void Close()
        {
            Destroy(gameObject);
        }
    }
}
