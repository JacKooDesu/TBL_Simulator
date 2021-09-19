using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TBL;
using TBL.Card;

namespace TBL.UI.GameScene
{
    public class PlayerData : MonoBehaviour
    {
        #region VARIABLES
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

        public void UpdateHero(NetworkPlayer player)
        {
            if (player.hero.HeroType == HeroType.Hidden)
                heroAvatar.sprite = ((NetworkRoomManager.singleton) as NetworkRoomManager).Judgement.heroList.hiddenAvatar;
            else
                heroAvatar.sprite = player.hero.Avatar;
        }

        public void UpdateStatus(NetworkPlayer player)
        {
            int blue = 0, red = 0, black = 0;
            foreach (int id in player.netCards)
            {
                CardSetting card = CardSetting.IDConvertCard((ushort)id);

                switch (card.CardColor)
                {
                    case CardColor.Black:
                        black++;
                        break;

                    case CardColor.Blue:
                        blue++;
                        break;

                    case CardColor.Red:
                        red++;
                        break;

                    default:
                        break;
                }
            }

            blueCount.text = blue.ToString();
            redCount.text = red.ToString();
            blackCount.text = black.ToString();

            handCardCount.text = player.netHandCard.Count.ToString();

            // lockBg.color = player.statu
        }
    }
}

