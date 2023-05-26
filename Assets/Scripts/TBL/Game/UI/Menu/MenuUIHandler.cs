using UnityEngine;
using UnityEngine.UI;

namespace TBL.UI.Menu
{
    using NetworkManager = Game.Networking.NetworkRoomManager;
    public class MenuUIHandler : MonoBehaviour
    {
        [SerializeField]
        Button host = default;
        [SerializeField]
        Button client = default;
        [SerializeField]
        InputField ip = default;
        [SerializeField]
        Button hostOnly = default;
        [SerializeField]
        InputField nameField = default;

        protected void Start()
        {
            var manager = NetworkManager.singleton;
            
            nameField.text = GameUtils.PlayerName;

            host.onClick.AddListener(manager.StartHost);

            client.onClick.AddListener(manager.StartClient);

            hostOnly.onClick.AddListener(manager.StartServer);

            nameField.onEndEdit.AddListener(s => GameUtils.PlayerName = s);

            ip.onValueChanged.AddListener(ip => manager.networkAddress = ip);
        }
    }
}
