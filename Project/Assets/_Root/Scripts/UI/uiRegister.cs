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

        bool validUsername = false;
        bool validPassword = false;
        bool validPasswordRegister = false;

        void Awake()
        {
            _addressFiled.onValueChanged.AddListener(v =>
            {
                NetworkManager.Instance.networkAddress = v.Length > 0 ? v : "localhost";
            }); 

            _username.onValueChanged.AddListener(str =>
            {
                validUsername = NetworkManager.Instance.ValidateUsername(str);
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

        public void UI_Register()
        {
            if (_password.text != _password2.text) {
                Debug.LogWarning("UI_Register: passwords dont match");
                return;
            }

            var info = new RegisterInfo(_username.text, _password.text, _email.text);

            NetworkManager.Instance.Register(info);
        }

        public void UI_Back()
        {
            uiCtrl.GotoLogin();
        }
    }
}