using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class uiRegister : MonoBehaviour
    {
        [SerializeField]
        public InputField _username;

        [SerializeField]
        public InputField _email;

        [SerializeField]
        public InputField _password;

        [SerializeField]
        public InputField _password2;

        [SerializeField]
        public GameObject _network;

        [SerializeField]
        public GameObject _onlogin;

        [SerializeField]
        public Button _register;

        SimpleAuthenticator auth;

        bool validUsername = false;
        bool validPassword = false;
        bool validPasswordRegister = false;

        void Awake()
        {
            auth = _network.GetComponent<SimpleAuthenticator>();
            auth.OnClientAuthenticate(NetworkClient.connection);
            auth.OnAuthSuccess += Auth_OnAuthSuccess;

            _password.contentType = InputField.ContentType.Password;
            _password2.contentType = InputField.ContentType.Password;

            _register.enabled = false;

            _username.onValueChanged.AddListener(str =>
            {
                validUsername = auth.ValidateUsername(str);
                _username.textComponent.color = validUsername ? Color.white : Color.red;
            });

            _password2.onValueChanged.AddListener(str =>
            {
                validPasswordRegister = validPassword && _password2.text == _password.text;
                _password2.textComponent.color = validPasswordRegister ? Color.white : Color.red;
                _register.enabled = validPasswordRegister;
            });

            _password.onValueChanged.AddListener(str =>
            {
                validPassword = _password.text.Length > 2;
                validPasswordRegister = validPassword && _password2.text == _password.text;
                _password.textComponent.color = validPassword ? Color.white : Color.red;
                _password2.textComponent.color = validPasswordRegister ? Color.white : Color.red;
                _register.enabled = validPasswordRegister;
            });
        }

        void Auth_OnAuthSuccess(object sender, System.EventArgs e)
        {
            Debug.Log("Auth_OnAuthSuccess");
            gameObject.SetActive(false);
            _onlogin?.SetActive(true);
        }

        public void UI_Login()
        {
            auth.SetNextActionLogin(_username.text, _password.text);
            auth.OnClientAuthenticate(NetworkClient.connection);
        }

        public void UI_Register()
        {
            if (_password.text == _password2.text) {
                auth.SetNextActionRegister(_username.text, _password.text);
                auth.OnClientAuthenticate(NetworkClient.connection);
            }
        }

        public void UI_Back()
        {
            uiCtrl.GotoLogin();
        }
    }
}