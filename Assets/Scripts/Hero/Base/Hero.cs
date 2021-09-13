using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBL.Card;

namespace TBL
{
    public class Hero : MonoBehaviour
    {
        [SerializeField, Header("角色名")]
        protected string heroName = "None";
        [SerializeField]
        protected Sprite avatar;

        [SerializeField, Header("角色屬性")]
        protected HeroType heroType;
        public HeroType HeroType
        {
            get => heroType;
        }
        [SerializeField]
        protected HeroGender gender;
        public HeroGender Gender
        {
            get => gender;
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

