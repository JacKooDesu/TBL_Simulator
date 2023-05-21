using UnityEngine;

namespace TBL.Game.Sys
{
    /// <summary>
    /// 資源管理，伺服端使用操作所有遊戲物件。
    /// </summary>
    public class Manager : MonoBehaviour
    {
        [SerializeField] DeckSetting deckSetting;
        [SerializeField] Deck deck;
        public Deck Deck => deck;

        void Start()
        {
            deck.Init(deckSetting);
        }
    }
}