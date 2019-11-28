using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class uiRegister : MonoBehaviour
    {
        [SerializeField]
        private InputField _addressFiled;

        [SerializeField]
        public InputField _userField;

        [SerializeField]
        public InputField _email;

        [SerializeField]
        public InputField _passwordField;

        [SerializeField]
        public InputField _password2;

        bool validUser = false;
        bool validPassword = false;
        bool validPasswordRegister = false;

        private void Awake()
        {
            _addressFiled.onValueChanged.AddListener(v => {
                NetworkManager.Instance.networkAddress = v.Length > 0 ? v : "localhost";
            });

            _userField.onValueChanged.AddListener(username => {
                validUser = NetworkManager.Instance.ValidateUsername(username);
                _userField.textComponent.color = validUser ? Color.white : Color.red;
            });

            _password2.onValueChanged.AddListener(passwordSame => {
                validPasswordRegister = validPassword && passwordSame == _passwordField.text;
                _password2.textComponent.color = validPasswordRegister ? Color.white : Color.red;
            });

            _passwordField.onValueChanged.AddListener(password => {
                validPassword = NetworkManager.Instance.ValidatePassword(password);
                validPasswordRegister = validPassword && _password2.text == password;
                _passwordField.textComponent.color = validPassword ? Color.white : Color.red;
                _password2.textComponent.color = validPasswordRegister ? Color.white : Color.red;
            });
        }

        public void UI_Register()
        {
            if (!validUser) {
                Debug.LogWarning("UI_Register: invalid username");
                return;
            }

            if (!validPassword) {
                Debug.LogWarning("UI_Register: invalid password");
                return;
            }

            if (!validPasswordRegister) {
                Debug.LogWarning("UI_Register: passwords dont match");
                return;
            }

            var info = new RegisterRequestMessage(_userField.text, _passwordField.text, _email.text);

            NetworkManager.Instance.Register(info);
        }

        public void UI_Back()
        {
            uiCtrl.GotoLogin();
        }
    }
}
