using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace TBL.Utils
{
    public class SmartEventTrigger : MonoBehaviour
    {
        public record TriggerSetting(EventTriggerType type, Action<BaseEventData> action);
        EventTrigger et;
        Func<bool> destroyCheck;

        public static SmartEventTrigger Bind(
            GameObject target,
            Func<bool> destroyCheck = null,
            params TriggerSetting[] settings)
        {
            var result = target.AddComponent<SmartEventTrigger>();
            result.et = target.AddComponent<EventTrigger>();
            result.destroyCheck = destroyCheck ?? (() => false);

            result.CheckParent();
            foreach (var s in settings)
                result.Add(s);

            return result;
        }

        void CheckParent()
        {
            ScrollRect sr;
            if ((sr = transform.GetComponentInParent<ScrollRect>()) == null)
                return;

            Add(new(EventTriggerType.Scroll, e => sr.OnScroll(e as PointerEventData)));
            Add(new(EventTriggerType.BeginDrag, e => sr.OnBeginDrag(e as PointerEventData)));
            Add(new(EventTriggerType.EndDrag, e => sr.OnEndDrag(e as PointerEventData)));
            Add(new(EventTriggerType.Drag, e => sr.OnDrag(e as PointerEventData)));
        }

        public SmartEventTrigger Add(TriggerSetting setting, bool asDestroyCheck = false)
        {
            Action<BaseEventData> action = asDestroyCheck ?
            (_) =>
            {
                setting.action(_);
                ForceDestroy();
            }
            : setting.action;

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = setting.type;
            entry.callback.AddListener(action.Invoke);
            et.triggers.Add(entry);

            return this;
        }

        private void Update()
        {
            if (!destroyCheck())
                return;

            ForceDestroy();
        }

        public void ForceDestroy()
        {
            Destroy(et);
            Destroy(this);
        }
    }
}
