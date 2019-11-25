using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class uiLogin : MonoBehaviour
    {
        [SerializeField]
        Button _startClient;

        [SerializeField]
        Button _startHost;

        [SerializeField]
        Button _startServer;

        [SerializeField]
        Button _stopClient;

        [SerializeField]
        Button _stopHost;

        [SerializeField]
        Button _stopServer;

        [SerializeField]
        InputField _addressFiled;

        [SerializeField]
        InputField _userFiled;

        [SerializeField]
        InputField _passwordFiled;

        bool validUser = false;
        bool validPassword = false;
        bool _serverStarted = false;
        bool _clientStarted = false;

        void Awake() {
            MonoBehaviour awake = ChannelManager.Instance;

            NetworkManager.StartedClient += (s, e) => {
                _clientStarted = true;
                SetActiveButtons();
            };

            NetworkManager.StoppedClient += (s, e) => {
                _clientStarted = false;
                SetActiveButtons();
            };

            NetworkManager.StartedServer += (s, e) => {
                _serverStarted = true;
                SetActiveButtons();
            };

            NetworkManager.StoppedServer += (s, e) => {
                _serverStarted = false;
                SetActiveButtons();
            };

            _addressFiled.onValueChanged.AddListener(v => {
                NetworkManager.Instance.networkAddress = v.Length > 0 ? v : "localhost";
            });

            _userFiled.onValueChanged.AddListener(str => {
                validUser = NetworkManager.Instance.ValidateUsername(str);
                _userFiled.textComponent.color = validUser ? Color.white : Color.red;
            });

            _passwordFiled.onValueChanged.AddListener(str => {
                validPassword = NetworkManager.Instance.ValidatePassword(_passwordFiled.text);
                _passwordFiled.textComponent.color = validPassword ? Color.white : Color.red;
            });
        }

        public void SetActiveButtons() {
            _startClient.gameObject.SetActive(!_clientStarted);
            _stopClient.gameObject.SetActive(_clientStarted && !_serverStarted);
            _startHost.gameObject.SetActive(!_clientStarted && !_serverStarted);
            _stopHost.gameObject.SetActive(_clientStarted && _serverStarted);
            _startServer.gameObject.SetActive(!_serverStarted);
            _stopServer.gameObject.SetActive(!_clientStarted && _serverStarted);
        }

        public void UI_Login() {
            if (validUser && validPassword) {
                NetworkManager.Instance.Login(_userFiled.text, _passwordFiled.text);
            } else {
                // TODO add notification to user
            }
        }

        public void UI_Client() {
            NetworkManager.Instance.StartClient();
        }

        public void UI_Host() {
            NetworkManager.Instance.StartHost();
        }

        public void UI_Server() {
            NetworkManager.Instance.StartServer();
        }

        public void UI_StopClient() {
            NetworkManager.Instance.StopClient();
        }

        public void UI_StopHost() {
            NetworkManager.Instance.StopHost();
        }

        public void UI_StopServer() {
            NetworkManager.Instance.StopServer();
        }

        public void UI_GotoRegister() {
            uiCtrl.GotoRegister();
        }
    }
}
