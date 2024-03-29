﻿using Mirror;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections.Concurrent;

public class SimpleAuthenticator : NetworkAuthenticator
{
    [SerializeField]
    string _usernameRegex = "^[a-zA-Z][a-zA-Z0-9]{2,19}$";

    [SerializeField]
    string _passwordRegex = "^[a-zA-Z0-9@$!%*#?&]{3,20}$";

    [SerializeField]
    string _emailRegex = "^[a-zA-Z0-9@$!%*#?&]{2,20}$";

    public event EventHandler OnAuthSuccess;

    void Awake()
    {
        //for testing
        UserManager.CreateUser(new RegisterRequestMessage("qwe", "qwe", "qwe@qwe.qwe"));
        UserManager.CreateUser(new RegisterRequestMessage("asd", "asd", "asd@asd.asd"));
    }

    public void Login(string username, string password)
    {
        var msg = new LoginRequestMessage(username, password);
        NetworkClient.Send(msg);
    }

    public void Register(RegisterRequestMessage req)
    {
        NetworkClient.Send(req);
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
                    ValidatePassword(req.Password) &&
                    UserManager.VerifyUser(req.Username, req.Password);

        AuthConnection(conn, valid, req.Username);
    }

    void OnRegisterRequestMessage(NetworkConnection conn, RegisterRequestMessage req)
    {

        Debug.LogFormat("Register Request: {0} {1}", req.Username ?? "", req.Password ?? "");

        if (ValidateUsername(req.Username) &&
            ValidatePassword(req.Password) &&
            ValidateEmail(req.Email) &&
            UserManager.CreateUser(req) != null) {
            AuthConnection(conn, true, req.Username);
        } else {
            AuthConnection(conn, false);
        }
    }

    public bool ValidateUsername(string username)
    {
        return username != null && Regex.IsMatch(username, _usernameRegex);
    }

    public bool ValidatePassword(string password)
    {
        return password != null && Regex.IsMatch(password, _passwordRegex);
    }

    public bool ValidateEmail(string email)
    {
        return email != null && Regex.IsMatch(email, _emailRegex);
    }

    void AuthConnection(NetworkConnection conn, bool success, string token = null)
    {
        if (success) {
            conn.authenticationData = token;

            base.OnServerAuthenticated.Invoke(conn);

            if (UserManager.Connect(conn) is User user) {
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
}

public class LoginRequestMessage : IMessageBase
{
    public string Username { get; set; }
    public string Password { get; set; }

    public LoginRequestMessage() { }
    public LoginRequestMessage(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public void Deserialize(NetworkReader reader)
    {
        Username = reader.ReadString();
        Password = reader.ReadString();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.WriteString(Username);
        writer.WriteString(Password);
    }
}

public class RegisterRequestMessage : IMessageBase
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }

    public RegisterRequestMessage() { }
    public RegisterRequestMessage(string username, string password, string email)
    {
        Username = username;
        Password = password;
        Email = email;
    }

    public void Deserialize(NetworkReader reader)
    {
        Username = reader.ReadString();
        Password = reader.ReadString();
        Email = reader.ReadString();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.WriteString(Username);
        writer.WriteString(Password);
        writer.WriteString(Email);
    }
}

public class AuthenticationResponseMessage : IMessageBase
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }

    public AuthenticationResponseMessage() { }
    public AuthenticationResponseMessage(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public void Deserialize(NetworkReader reader)
    {
        IsSuccess = reader.ReadBoolean();
        Message = reader.ReadString();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.WriteBoolean(IsSuccess);
        writer.WriteString(Message);
    }
}

