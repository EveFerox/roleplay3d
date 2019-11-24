using Mirror;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Ceras;
using UnityEngine;

public class SimpleAuthenticator : NetworkAuthenticator
{
    readonly Dictionary<string, string> DataBase = new Dictionary<string, string>();

    [SerializeField]
    string _usernameRegex = "^[a-zA-Z][a-zA-Z0-9]{2,19}$";

    public event EventHandler OnAuthSuccess;

    void Awake()
    {
        //for testing
        DataBase.Add("qwe", "qwe");
        DataBase.Add("asd", "asd");
    }

    public void Login(string username, string password)
    {
        var msg = new LoginRequestMessage(username, password);
        NetworkClient.Send(msg);
    }

    public void Register(RegisterInfo info)
    {
        var msg = new RegisterRequestMessage(info);
        NetworkClient.Send(msg);
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

    public override void OnServerAuthenticate(NetworkConnection conn) { }

    public override void OnClientAuthenticate(NetworkConnection conn) { }

    void OnLoginRequestMessage(NetworkConnection conn, LoginRequestMessage req)
    {
        Debug.LogFormat("Login Request: {0} {1}", req.Username, req.Password);

        var valid = ValidateUsername(req.Username) &&
                    DataBase.TryGetValue(req.Username, out var hash) &&
                    hash == req.Password;

        AuthConnection(conn, valid, req.Username);
    }

    void OnRegisterRequestMessage(NetworkConnection conn, RegisterRequestMessage req)
    {
        var info = req.RegisterInfo;

        Debug.LogFormat("Register Request: {0} {1}", info.Username ?? "", info.Password ?? "");

        if (ValidateUsername(info.Username) && !DataBase.ContainsKey(info.Username)) {
            DataBase.Add(info.Username, info.Password);
            AuthConnection(conn, true, info.Username);
        } else {
            AuthConnection(conn, false);
        }
    }

    public bool ValidateUsername(string username)
    {
        return username != null && Regex.IsMatch(username, _usernameRegex);
    }

    void AuthConnection(NetworkConnection conn, bool success, string token = null)
    {
        if (success) {
            conn.authenticationData = token;

            base.OnServerAuthenticated.Invoke(conn);

            if (UserManager.Register(conn) is User user) {
                ChannelManager.SubscribeToGlobal(user);
            } else {
                AuthConnection(conn, false);
                return;
            }

            conn.Send(new AuthenticationResponseMessage(true, "Success"));
        } else {
            conn.Send(new AuthenticationResponseMessage(false, "Invalid Credentials"));
            conn.isAuthenticated = false;
        }
    }

    void OnAuthenticationResponseMessage(NetworkConnection conn, AuthenticationResponseMessage res)
    {
        if (res.IsSuccess) {
            Debug.LogFormat("Authentication Response: {0}", res.Message);

            base.OnClientAuthenticated.Invoke(conn);

            OnAuthSuccess?.Invoke(this, EventArgs.Empty);
        } else {
            Debug.LogErrorFormat("Authentication Response: {0}", res.Message);

            conn.isAuthenticated = false;
            conn.Disconnect();
        }
    }

    public class LoginRequestMessage : DefaultMessageBase
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginRequestMessage() { }
        public LoginRequestMessage(string username, string password)
        {
            Username = username;
            Password = password;
        }

        protected override void CopyFrom(object obj)
        {
            if (obj is LoginRequestMessage v) {
                Username = v.Username;
                Password = v.Password;
            }
        }
    }

    public class RegisterRequestMessage : DefaultMessageBase
    {
        [Include]
        public RegisterInfo RegisterInfo { get; set; }
        
        public RegisterRequestMessage() { }
        public RegisterRequestMessage(RegisterInfo registerInfo)
        {
            RegisterInfo = registerInfo;
        }

        protected override void CopyFrom(object obj)
        {
            if (obj is RegisterRequestMessage v) {
                RegisterInfo = v.RegisterInfo;
            }
        }
    }

    public class AuthenticationResponseMessage : DefaultMessageBase
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        
        public AuthenticationResponseMessage() { }
        public AuthenticationResponseMessage(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        protected override void CopyFrom(object obj)
        {
            if (obj is AuthenticationResponseMessage v) {
                IsSuccess = v.IsSuccess;
                Message = v.Message;
            }
        }
    }
}

public struct RegisterInfo
{
    [Include]
    public string Username { get; set; }
    [Include]
    public string Password { get; set; }
    [Include]
    public string Email { get; set; }

    public RegisterInfo(string username, string password, string email)
    {
        Username = username;
        Password = password;
        Email = email;
    }
}