using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class uiLogin : MonoBehaviour
{

    [SerializeField]
    public InputField _username;

    [SerializeField]
    public InputField _password;

    [SerializeField]
    public InputField _passwordRegister;

    [SerializeField]
    public GameObject _network;


    [SerializeField]
    public GameObject _onlogin;


    private SimpleAuthenticator auth;


    private void Awake()
    {
        auth = _network.GetComponent<SimpleAuthenticator>();
        auth.OnClientAuthenticate(NetworkClient.connection);
        auth.OnAuthSuccess += Auth_OnAuthSuccess;

        _password.contentType = InputField.ContentType.Password;
        _passwordRegister.contentType = InputField.ContentType.Password;
    }

    private void Auth_OnAuthSuccess(object sender, System.EventArgs e)
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
        if (_password.text == _passwordRegister.text)
        {
            auth.SetNextActionRegister(_username.text, _password.text, _passwordRegister.text);
            auth.OnClientAuthenticate(NetworkClient.connection);
        }
    }
}
