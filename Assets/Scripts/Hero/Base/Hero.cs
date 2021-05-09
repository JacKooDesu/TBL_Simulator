using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TBL
{
    public class Hero : MonoBehaviour
    {
        [SerializeField, Header("角色名")]
        protected string heroName = "None";


        [SerializeField, Header("角色屬性")]
        protected HeroType heroType;
        public HeroType HeroType
        {
            get => heroType;
        }

        [SerializeField, Header("使用者")]
        protected NetworkPlayer playerStatus;
        public NetworkPlayer PlayerStatus
        {
            set { playerStatus = value; }
            get => playerStatus;
        }

        public virtual void OnGetCard(CardObject card)
        {
            
        }
    }
}

