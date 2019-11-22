using Mirror.Authenticators;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class uiLogin : MonoBehaviour
    {
        [SerializeField]
        InputField _addressFiled;

        [SerializeField]
        InputField _userFiled;

        [SerializeField]
        InputField _passwordFiled;

        [SerializeField]
        BasicAuthenticator _auth;

        [SerializeField]
        NetworkManager _manager;

        void Awake()
        {
            _manager = FindObjectOfType<NetworkManager>();

            if (_auth != null) {
                _userFiled.text = _auth.username;
                _passwordFiled.text = _auth.password;
            }
        }

        void UpdateAuth()
        {
            if (_auth != null) {
                _auth.username = _userFiled.text;
                _auth.password = _passwordFiled.text;
            }
            _manager.networkAddress = _addressFiled.text.Length > 0 ? _addressFiled.text : "localhost";
        }

        public void UI_Login()
        {
            UpdateAuth();

            _manager.StartClient();
        }

        public void UI_Host()
        {
            UpdateAuth();

            _manager.StartHost();
        }

        public void UI_Server()
        {
            UpdateAuth();

            _manager.StartServer();
        }
    }
}
