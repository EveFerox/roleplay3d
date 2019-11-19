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

    [SerializeField]
    public Button _register;


    private SimpleAuthenticator auth;

    private bool validUsername = false;
    private bool validPassword = false;
    private bool validPasswordRegister = false;

    private void Awake()
    {
        auth = _network.GetComponent<SimpleAuthenticator>();
        auth.OnClientAuthenticate(NetworkClient.connection);
        auth.OnAuthSuccess += Auth_OnAuthSuccess;

        _password.contentType = InputField.ContentType.Password;
        _passwordRegister.contentType = InputField.ContentType.Password;

        _register.enabled = false;

        _username.onValueChanged.AddListener(str =>
        {
            validUsername = auth.ValidateUsername(str);
            _username.textComponent.color = validUsername ? Color.white : Color.red;
        });

        _passwordRegister.onValueChanged.AddListener(str =>
        {
            validPasswordRegister = validPassword && _passwordRegister.text == _password.text;
            _passwordRegister.textComponent.color = validPasswordRegister ? Color.white : Color.red;
            _register.enabled = validPasswordRegister;
        });

        _password.onValueChanged.AddListener(str =>
        {
            validPassword = _password.text.Length > 2;
            validPasswordRegister = validPassword && _passwordRegister.text == _password.text;
            _password.textComponent.color = validPassword ? Color.white : Color.red;
            _passwordRegister.textComponent.color = validPasswordRegister ? Color.white : Color.red;
            _register.enabled = validPasswordRegister;
        });
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
