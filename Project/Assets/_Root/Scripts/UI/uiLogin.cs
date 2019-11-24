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

        void Awake()
        {
            _addressFiled.onValueChanged.AddListener(v =>
            {
                NetworkManager.Instance.networkAddress = v.Length > 0 ? v : "localhost";
            }); 
        }

        public void UI_Login()
        {
            NetworkManager.Instance.Login(_userFiled.text, _passwordFiled.text);
        }

        public void UI_Host()
        {
            NetworkManager.Instance.StartHost();
        }

        public void UI_Server()
        {
            NetworkManager.Instance.StartServer();
        }

        public void UI_GotoRegister()
        {
            uiCtrl.GotoRegister();
        }
    }
}
