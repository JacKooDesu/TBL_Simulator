using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TBL.UI.GameScene
{
    public class OptionObject : MonoBehaviour
    {
        [SerializeField] Text text;
        EventTrigger eventTrigger;
        Option option;

        private void Awake()
        {
            eventTrigger = GetComponent<EventTrigger>();
        }

        public void Init(Option option)
        {
            this.option = option;

            text.text = option.str;
            EventTrigger.Entry click;
            eventTrigger.triggers.Add(
                click = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick }
            );
            click.callback.AddListener((e) => option.onSelect.Invoke());
        }
    }
}
