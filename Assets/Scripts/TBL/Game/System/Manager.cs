using UnityEngine;
namespace TBL.Game.Sys
{
    using Setting;
    using NetworkManager = Networking.NetworkRoomManager;
    using LocalManager = Networking.LocalManager;

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
            if ((NetworkManager.singleton != null &&
                (NetworkManager.singleton.Me() != null && NetworkManager.singleton.Me().isClient)) ||
                LocalManager.Singleton == null)
                return;

            IPlayerStandalone[] standalones = null!;
            if (NetworkManager.singleton)
            {
                standalones = NetworkManager.singleton.players.ToArray();
            }
            else if (LocalManager.Singleton)
            {
                standalones = LocalManager.Singleton.Players.ToArray();
            }

            deck.Init(deckSetting)
                .sleeping.AddRange(deck.CardDatas);
            playerList.Init(teamSetting, heroSetting, standalones);
        }
    }
}