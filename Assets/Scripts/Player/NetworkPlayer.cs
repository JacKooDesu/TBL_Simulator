using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace TBL
{
    public class NetworkPlayer : MonoBehaviour
    {
        [SerializeField, Header("手牌")]
        List<CardObject> handCards = new List<CardObject>();

        [SerializeField, Header("情報")]
        List<CardObject> cards = new List<CardObject>();
    }
}

