using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class uiRegister : MonoBehaviour
    {
        [SerializeField]
        InputField _addressFiled;

        [SerializeField]
        public InputField _username;

        [SerializeField]
        public InputField _email;

        [SerializeField]
        public InputField _password;

        [SerializeField]
        public InputField _password2;

        [SerializeField]
        public GameObject _onlogin;
        
        [SerializeField]
        SimpleAuthenticator _auth;

        bool validUsername = false;
        bool validPassword = false;
        bool validPasswordRegister = false;

        void Awake()
        {
            _auth.OnAuthSuccess += Auth_OnAuthSuccess;

            _addressFiled.onValueChanged.AddListener(v =>
            {
                NetworkManager.Instance.networkAddress = v.Length > 0 ? v : "localhost";
            }); 

            _username.onValueChanged.AddListener(str =>
            {
                validUsername = _auth.ValidateUsername(str);
                _username.textComponent.color = validUsername ? Color.white : Color.red;
            });

            _password2.onValueChanged.AddListener(str =>
            {
                validPasswordRegister = validPassword && _password2.text == _password.text;
                _password2.textComponent.color = validPasswordRegister ? Color.white : Color.red;
            });

            _password.onValueChanged.AddListener(str =>
            {
                validPassword = _password.text.Length > 2;
                validPasswordRegister = validPassword && _password2.text == _password.text;
                _password.textComponent.color = validPassword ? Color.white : Color.red;
                _password2.textComponent.color = validPasswordRegister ? Color.white : Color.red;
            });
        }

        void Auth_OnAuthSuccess(object sender, System.EventArgs e)
        {
            Debug.Log("Auth_OnAuthSuccess");
            gameObject.SetActive(false);
            _onlogin?.SetActive(true);
        }

        public void UI_Register()
        {
            if (_password.text != _password2.text) {
                Debug.LogWarning("UI_Register: passwords dont match");
                return;
            }

            _auth.SetNextActionRegister(_username.text, _password.text);
            _auth.OnClientAuthenticate(NetworkClient.connection);
        }

        public void UI_Back()
        {
            uiCtrl.GotoLogin();
        }
    }
}