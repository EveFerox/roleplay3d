using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    private readonly Dictionary<string, User> DataBase = new Dictionary<string, User>();

    public User GetUser(NetworkConnection conn)
    {
        if (conn.isAuthenticated && conn.authenticationData is string username && DataBase.TryGetValue(username, out var user))
        {
            return user;
        }
        return null;
    }

    public User Register(NetworkConnection conn)
    {
        if (conn.isAuthenticated && conn.authenticationData is string username)
        {
            var user = new User(username) { Connection = conn };
            DataBase.Add(username, user);
            user.OnDisconnect += (s, e) => DataBase.Remove(user.Username);
            return user;
        }
        return null;
    }
}

public class User
{
    public event EventHandler OnDisconnect;

    public readonly string Username;
    public NetworkConnection Connection { get; set; }

    internal void DisconnectedInternal()
    {
        OnDisconnect?.Invoke(this, EventArgs.Empty);
    }

    public User(string username)
    {
        Username = username;
    }

    public override int GetHashCode()
    {
        return Username.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return obj is User user && user.Username == Username;
    }
}

