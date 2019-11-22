using Mirror;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class SimpleAuthenticator : NetworkAuthenticator
{
    readonly Dictionary<string, string> DataBase = new Dictionary<string, string>();

    IMessageBase authMessage;

    [SerializeField]
    string _usernameRegex = "^[a-zA-Z][a-zA-Z0-9]{2,19}$";

    public event EventHandler OnAuthSuccess;

    public bool SetNextActionLogin(string username, string password)
    {
        if (!ValidateUsername(username)) return false;
        authMessage = new LoginRequestMessage {Username = username, Password = password};
        return true;
    }

    public bool SetNextActionRegister(string username, string password)
    {
        if (!ValidateUsername(username)) return false;
        authMessage = new RegisterRequestMessage {Username = username, Password = password};
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

    public override void OnServerAuthenticate(NetworkConnection conn) { }

    public override void OnClientAuthenticate(NetworkConnection conn)
    {
        switch (authMessage) {
            case LoginRequestMessage login:
                conn.Send(login);
                break;
            case RegisterRequestMessage register:
                conn.Send(register);
                break;
        }

        authMessage = null;
    }

    void OnLoginRequestMessage(NetworkConnection conn, LoginRequestMessage req)
    {
        Debug.LogFormat("Login Request: {0} {1}", req.Username, req.Password);

        AuthConnection(conn, ValidateUsername(req.Username) &&
                             DataBase.TryGetValue(req.Username, out var hash) &&
                             hash == req.Password,
            req.Username);
    }

    void OnRegisterRequestMessage(NetworkConnection conn, RegisterRequestMessage req)
    {
        Debug.LogFormat("Register Request: {0} {1}", req.Username ?? "", req.Password ?? "");

        if (ValidateUsername(req.Username) && !DataBase.ContainsKey(req.Username)) {
            DataBase.Add(req.Username, req.Password);
            AuthConnection(conn, true, req.Username);
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

            conn.Send(new AuthenticationResponseMessage
            {
                IsSuccess = true,
                Message = "Success",
            });
        } else {
            conn.Send(new AuthenticationResponseMessage
            {
                IsSuccess = false,
                Message = "Invalid Credentials"
            });
            conn.isAuthenticated = false;
            Invoke(nameof(conn.Disconnect), 1);
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
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        protected override void CopyFrom(object obj)
        {
            if (obj is RegisterRequestMessage v) {
                Username = v.Username;
                Password = v.Password;
                Email = v.Email;
            }
        }
    }

    public class AuthenticationResponseMessage : DefaultMessageBase
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        protected override void CopyFrom(object obj)
        {
            if (obj is AuthenticationResponseMessage v) {
                IsSuccess = v.IsSuccess;
                Message = v.Message;
            }
        }
    }
}