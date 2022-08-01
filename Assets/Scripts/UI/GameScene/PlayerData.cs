using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TBL;
using TBL.Card;

namespace TBL.UI.GameScene
{
    using static CardAttributeHelper;
    public class PlayerData : MonoBehaviour
    {
        #region VARIABLES
        public NetworkPlayer player;

        [Header("角色圖")]
        public Image heroAvatar;

        [Header("技能")]
        public Text skill1;
        public Text skill2;
        public Text skill3;

        [Header("檯面狀態")]
        public Text blueCount;
        public Text redCount;
        public Text blackCount;

        [Header("玩家狀態")]
        public Image skipBg;
        public Image lockBg;
        public Color activeColor;
        public Color inactiveColor;

        [Header("手牌")]
        public Text handCardCount;

        [Header("任務")]
        public Text greenMission;
        #endregion

        private void Start()
        {
            UpdateStatus();
            // UpdateHero();
        }

        public void UpdateHero()
        {
            if (player.hero.isHiding)
                heroAvatar.sprite = ((NetworkRoomManager.singleton) as NetworkRoomManager).Judgement.heroList.hiddenAvatar;
            else
                heroAvatar.sprite = player.hero.Avatar;
        }

        public void UpdateStatus()
        {
            if (player == null)
                return;

            int blue = 0, red = 0, black = 0;
            foreach (int id in player.netCards)
            {
                if (Compare(id, Black))
                    black++;

                if (Compare(id, Blue))
                    blue++;

                if (Compare(id, Red))
                    red++;
            }

            blueCount.text = blue.ToString();
            redCount.text = red.ToString();
            blackCount.text = black.ToString();

            handCardCount.text = player.netHandCards.Count.ToString();

            lockBg.color = player.isLocked ? activeColor : inactiveColor;
            skipBg.color = player.isSkipped ? activeColor : inactiveColor;
        }


    }
}

