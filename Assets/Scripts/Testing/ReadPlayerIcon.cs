using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace TBL.Testing
{
    public class ReadPlayerIcon : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnValueChanged))]
        public int value;

        public Text UIText;

        private void Update()
        {
            UIText.text = value.ToString();
        }

        public void OnValueChanged(int oldValue, int newValue)
        {
            RpcChangeValue(newValue);
        }

        [ClientRpc]
        public void RpcChangeValue(int i)
        {
            value = i;
        }
    }
}
