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
        public GameObject optionPrefab;

        public Menu InitCustomMenu(List<string> options, List<UnityAction> actions)
        {
            Clear();
            
            if (options.Count == 0)
                return null;

            for (int i = 0; i < options.Count; ++i)
            {
                int x = i;
                var s = options[i];
                OptionObject option = Instantiate(optionPrefab, transform).GetComponent<OptionObject>();
                option.GetComponentInChildren<Text>().text = s;

                JacDev.Utils.EventBinder.Bind(
                    option.GetComponent<EventTrigger>(),
                    EventTriggerType.PointerClick,
                    (e) =>
                    {
                        actions[x].Invoke();
                        Clear();
                        print($"選擇 {s}");
                        gameObject.SetActive(true);
                    });
            }

            gameObject.SetActive(true);

            return this;
        }

        public Menu AddCustomOption(string text, UnityAction action)
        {
            OptionObject option = Instantiate(optionPrefab, transform).GetComponent<OptionObject>();
            option.GetComponentInChildren<Text>().text = text;
            JacDev.Utils.EventBinder.Bind(
                option.GetComponent<EventTrigger>(),
                EventTriggerType.PointerClick,
                (e) =>
                {
                    action.Invoke();
                });

            return this;
        }

        public void Clear()
        {
            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            gameObject.SetActive(false);
        }
    }
}