using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TBL.UI.GameScene
{
    public class Menu : TempMenuBase
    {
        public OptionObject optionPrefab;
        public Transform contentParent;

        public override void Init(List<Option> options, int defaultIndex = -1)
        {
            base.Init(options, defaultIndex);
            this.options.Add(
                new Option
                {
                    str = "取消",
                    onSelect = Close
                }
            );

            foreach (var o in this.options)
            {
                var optionObj = Instantiate(optionPrefab, contentParent);
                optionObj.Init(o);
            }
        }
    }
}