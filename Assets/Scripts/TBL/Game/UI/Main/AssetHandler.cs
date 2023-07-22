using UnityEngine;

namespace TBL.Game.Sys
{
    public class AssetHandler : MonoBehaviour
    {
        public static AssetHandler Instance { get; private set; }

        [SerializeField] HeroAsset _heroAssets;
        public HeroAsset Hero => _heroAssets;

        [SerializeField] TeamAsset _teamAssets;
        public TeamAsset Team => _teamAssets;

        void Awake()
        {
            Instance = this;
        }

        void OnDestroy() => Instance = null;
    }
}