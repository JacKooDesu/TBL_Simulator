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

    }

    // Update is called once per frame
    void Update()
    {
        rightClick = false;
        if (Input.GetMouseButtonDown(2))
            rightClick = true;
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
