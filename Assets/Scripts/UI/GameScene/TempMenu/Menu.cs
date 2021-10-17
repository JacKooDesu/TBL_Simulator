using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TBL.UI.GameScene
{
    public class Menu : MonoBehaviour
    {
        public GameObject optionPrefab;

        public void InitCardMenu(List<int> cardList, UnityAction<int> action)
        {
            foreach (int i in cardList)
            {
                Option option = Instantiate(optionPrefab).GetComponent<Option>();
                option.GetComponentInChildren<Text>().text = Card.CardSetting.IDConvertCard(i).CardName;
                JacDev.Utils.EventBinder.Bind(
                    option.GetComponent<EventTrigger>(),
                    EventTriggerType.PointerClick,
                    (e) =>
                    {
                        action.Invoke(i);
                        Clear();
                        print($"選擇 {i}");
                        gameObject.SetActive(true);
                    });

                option.transform.SetParent(transform);
            }

            gameObject.SetActive(true);
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