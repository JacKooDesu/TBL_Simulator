using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VersionText : MonoBehaviour
{
    public Text text;
    public string githubLink;
    private void Start()
    {
        text.text = $"GitHub\nv {Application.version}";
        var eventTrigger = gameObject.AddComponent<EventTrigger>();
        var trigger = new EventTrigger.Entry();
        trigger.eventID = EventTriggerType.PointerClick;
        trigger.callback.AddListener((e) =>
        {
            Application.OpenURL(githubLink);
        });
        eventTrigger.triggers.Add(trigger);
    }
}
