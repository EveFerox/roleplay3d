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
        SimpleAuthenticator _auth;

        NetworkManager _manager;

        void Awake()
        {
            _manager = FindObjectOfType<NetworkManager>();
        }

        void UpdateAuth()
        {
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

        public void UI_GotoRegister()
        {
            uiCtrl.GotoRegister();
        }
    }
}
