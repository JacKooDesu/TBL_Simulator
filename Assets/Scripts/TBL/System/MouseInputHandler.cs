using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseInputHandler : MonoBehaviour
{
    static bool rightClick, rightClickLast;
    public static bool rightClickInvoke;
    public static event UnityAction rightClickEvent;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        rightClickEvent += () => print("Right Clicked");
    }

    // Update is called once per frame
    void Update()
    {
        rightClick = false;
        if (Input.GetMouseButtonDown(1))
        {
            rightClick = true;
        }

    }

    private void LateUpdate()
    {
        rightClickInvoke = false;
        if (rightClickLast != rightClick)
        {
            rightClickLast = rightClick;
            if (rightClickLast)
            {
                rightClickInvoke = true;
                rightClickEvent.Invoke();
            }
        }
    }
}
