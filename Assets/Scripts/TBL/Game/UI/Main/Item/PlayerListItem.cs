using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace TBL.Game.UI.Main
{
    using TBL.Game;
    public class PlayerListItem : MonoBehaviour
    {
        [SerializeField] Player player;

        [Header("角色圖")]
        [SerializeField] Image heroAvatar;

        [Header("技能")]
        [SerializeField] TMP_Text skill1;
        [SerializeField] TMP_Text skill2;
        [SerializeField] TMP_Text skill3;

        [Header("檯面狀態")]
        [SerializeField] TMP_Text blueCount;
        [SerializeField] TMP_Text redCount;
        [SerializeField] TMP_Text blackCount;

        [Header("玩家狀態")]
        [SerializeField] Image skipBg;
        [SerializeField] Image lockBg;
        [SerializeField] Color activeColor;
        [SerializeField] Color inactiveColor;

        [Header("手牌")]
        [SerializeField] TMP_Text handCardCount;

        [Header("任務")]
        [SerializeField] TMP_Text greenMission;

        public void Init(Player player)
        {
            this.player = player;
        }

        public void Bind()
        {
            player.CardStatus.OnChanged += UpdateCard;
            player.HeroStatus.OnChanged += UpdateHero;
            player.ReceiverStatus.OnChanged += UpdateReciver;
        }

        void UpdateCard(CardStatus status)
        {
            handCardCount.text = $"{status.Hand.Count}";
            var table = CardCollection.FromList(status.Table);
            redCount.text = $"{table.Red().Count}";
            blueCount.text = $"{table.Blue().Count}";
            blackCount.text = $"{table.Black().Count}";
        }

        void UpdateHero(HeroStatus status)
        {
            // heroAvatar.
        }

        void UpdateReciver(ReceiveEnum status)
        {
            
        }
    }
}