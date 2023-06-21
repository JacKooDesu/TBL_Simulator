using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
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

        protected async void Start()
        {
            NetworkManager manager = NetworkManager.singleton as NetworkManager;
            await UniTask.WaitUntil(() => (manager = NetworkManager.singleton as NetworkManager) != null);

            nameField.text = GameUtils.PlayerName;

            host.onClick.AddListener(manager.StartHost);

            client.onClick.AddListener(manager.StartClient);

            hostOnly.onClick.AddListener(manager.StartServer);

            nameField.onEndEdit.AddListener(s => GameUtils.PlayerName = s);

            ip.onValueChanged.AddListener(ip => manager.networkAddress = ip);
        }
    }
}
