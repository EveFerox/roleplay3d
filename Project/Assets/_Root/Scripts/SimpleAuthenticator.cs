using Mirror;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[RequireComponent(typeof(UserManager))]
public class SimpleAuthenticator : NetworkAuthenticator
{
    private readonly Dictionary<string, string> DataBase = new Dictionary<string, string>();

    private IMessageBase authMessage;

    private UserManager users;
    private ChannelManager channels;

    [SerializeField]
    private string usernameRegex = "^[a-zA-Z][a-zA-Z0-9]{3,19}$";

    public event EventHandler OnAuthSuccess;

    void Awake()
    {
        users = GetComponent<UserManager>();
        channels = GetComponent<ChannelManager>();
    }

    [Client]
    public bool SetNextActionLogin(string username, string password)
    {
        if (!ValidateUsername(username)) return false;
        authMessage = new LoginRequestMessage { Username = username, Password = password };
        return true;
    }

    [Client]
    public bool SetNextActionRegister(string username, string password, string password2)
    {
        if (!ValidateUsername(username)) return false;
        authMessage = new RegisterRequestMessage { Username = username, Password = password, Password2 = password2 };
        return true;
    }

    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler<LoginRequestMessage>(OnLoginRequestMessage, false);
        NetworkServer.RegisterHandler<RegisterRequestMessage>(OnRegisterRequestMessage, false);
    }

    public override void OnStartClient()
    {
        NetworkClient.RegisterHandler<AuthenticationResponseMessage>(OnAuthenticationResponseMessage, false);
    }

    public override void OnServerAuthenticate(NetworkConnection conn)
    {

    }

    public override void OnClientAuthenticate(NetworkConnection conn)
    {
        if (authMessage is LoginRequestMessage login)
        {
            conn.Send(login);
        }

        if (authMessage is RegisterRequestMessage register)
        {
            conn.Send(register);
        }

        authMessage = null;
    }

    [Server]
    private void OnLoginRequestMessage(NetworkConnection conn, LoginRequestMessage req)
    {
        Debug.LogFormat("Login Request: {0} {1}", req.Username, req.Password);

        AuthConnection(conn, ValidateUsername(req.Username) && 
            DataBase.TryGetValue(req.Username, out var hash) && 
            hash == req.Password,
            req.Username);
    }

    [Server]
    private void OnRegisterRequestMessage(NetworkConnection conn, RegisterRequestMessage req)
    {
        Debug.LogFormat("Register Request: {0} {1}", req.Username ?? "", req.Password ?? "");

        if (ValidateUsername(req.Username) && !DataBase.ContainsKey(req.Username))
        {
            DataBase.Add(req.Username, req.Password);
            AuthConnection(conn, true, req.Username);
        }
        else
        {
            AuthConnection(conn, false);
        }
    }

    private bool ValidateUsername(string username)
    {
        return username != null && Regex.IsMatch(username, usernameRegex);
    }

    [Server]
    private void AuthConnection(NetworkConnection conn, bool success, string token = null)
    {
        if (success)
        {
            conn.authenticationData = token;

            base.OnServerAuthenticated.Invoke(conn);

            if (users.Register(conn) is User user)
            {
                channels.SubscribeToGlobal(user);
            }
            else
            {
                AuthConnection(conn, false);
                return;
            }

            conn.Send(new AuthenticationResponseMessage
            {
                Success = true,
                Message = "Success",
            });
        }
        else
        {
            conn.Send(new AuthenticationResponseMessage
            {
                Success = false,
                Message = "Invalid Credentials"
            });
            conn.isAuthenticated = false;
            Invoke(nameof(conn.Disconnect), 1);
        }
    }

    [Client]
    private void OnAuthenticationResponseMessage(NetworkConnection conn, AuthenticationResponseMessage res)
    {
        if (res.Success)
        {
            Debug.LogFormat("Authentication Response: {0}", res.Message);

            base.OnClientAuthenticated.Invoke(conn);

            OnAuthSuccess?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Debug.LogErrorFormat("Authentication Response: {0}", res.Message);

            conn.isAuthenticated = false;
            conn.Disconnect();
        }
    }

    public class LoginRequestMessage : DefaultMessageBase
    {
        public string Username { get; set; }
        public string Password { get; set; }

        protected override void CopyFrom(object obj)
        {
            if (obj is LoginRequestMessage v)
            {
                Username = v.Username;
                Password = v.Password;
            }
        }
    }

    public class RegisterRequestMessage : DefaultMessageBase
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Password2 { get; set; }

        protected override void CopyFrom(object obj)
        {
            if (obj is RegisterRequestMessage v)
            {
                Username = v.Username;
                Password = v.Password;
                Password2 = v.Password2;
            }
        }
    }

    public class AuthenticationResponseMessage : DefaultMessageBase
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        protected override void CopyFrom(object obj)
        {
            if (obj is AuthenticationResponseMessage v)
            {
                Success = v.Success;
                Message = v.Message;
            }
        }
    }
}