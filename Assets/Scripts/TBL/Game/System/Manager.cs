using UnityEngine;
namespace TBL.Game.Sys
{
    using Setting;

    /// <summary>
    /// 資源管理，伺服端使用操作所有遊戲物件。
    /// </summary>
    public class Manager : MonoBehaviour
    {
        [SerializeField] DeckSetting deckSetting;
        [SerializeField] Deck deck = new Deck();
        public Deck Deck => deck;

        [SerializeField] TeamSetting teamSetting;
        [SerializeField] HeroSetting heroSetting;
        [SerializeField] PlayerList playerList = new PlayerList();
        public PlayerList PlayerList => playerList;

        void Start()
        {
            deck.Init(deckSetting)
                .sleeping.AddRange(deck.CardDatas);
            playerList.Init(2, teamSetting, heroSetting);
        }
    }
}