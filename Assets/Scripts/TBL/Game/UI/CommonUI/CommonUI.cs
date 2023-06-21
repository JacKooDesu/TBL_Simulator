using UnityEngine;

namespace TBL.Game.UI
{
    using Sys;
    public class CommonUI : MonoBehaviour, ISetupWith<IPlayerStandalone>
    {
        public static CommonUI Singleton { get; private set; }

        [SerializeField] BannerUI bannerUI;
        public BannerUI BannerUI => bannerUI;

        public void Setup(IPlayerStandalone res)
        {
            if (res != IPlayerStandalone.Me)
                return;

            res.PacketHandler.ChangePhasePacketEvent += ChangePacket;
        }

        void ChangePacket(Networking.ChangePhasePacket packet)
        {
            bannerUI.Show(
                Phase.Get(packet.PhaseType).PhaseName,
                Color.white * .8f,
                2f);
            CanvasGroup can = new();
        }
    }
}
