using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TBL.NetCanvas
{
    public class NetCanvasBinderBase : NetworkBehaviour
    {
        protected NetworkRoomManager manager;

        protected virtual void Start()
        {
            manager = GameObject.FindObjectOfType<NetworkRoomManager>();
        }

        protected void BindEvent<T>(T e, UnityAction action) where T : UnityEvent
        {
            e.AddListener(action);
        }
    }
}

