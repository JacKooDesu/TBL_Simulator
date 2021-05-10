using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBL.NetCanvas;

namespace TBL
{
    public class NetworkRoomPlayer : Mirror.NetworkRoomPlayer
    {
        [SerializeField]
        GameObject playerUIPrefab = default;
        GameObject playerUI;
        bool hasGenerateUI = false;

        private void Update()
        {
            if (!hasGenerateUI)
            {
                playerUI = Instantiate(playerUIPrefab);
                GameObject g = FindObjectOfType<RoomScene>().playerListUI;
                playerUI.transform.SetParent(g.transform);
                playerUI.transform.localScale = Vector3.one;
                hasGenerateUI = true;
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Destroy(playerUI);
        }
    }
}

