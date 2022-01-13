using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

namespace JacDev.Utils
{
    public class EventBinder : MonoBehaviour
    {
        public static void Bind(EventTrigger targetTrigger, EventTriggerType triggerType, UnityAction<BaseEventData> action)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = triggerType;
            entry.callback.AddListener(action);
            targetTrigger.triggers.Add(entry);
        }

        public static void Bind(EventTrigger targetTrigger, EventTriggerType triggerType, System.Action action)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = triggerType;
            entry.callback.AddListener((e) => action.Invoke());
            targetTrigger.triggers.Add(entry);
        }

        public static IEnumerator FunctionDelay(float delay, System.Action action)
        {
            yield return new WaitForSeconds(delay);
            action.Invoke();
        }
    }
}

