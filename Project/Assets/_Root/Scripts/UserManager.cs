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
            if (DataBase.TryGetValue(username, out var user))
            {
                if (user.Connection == null)
                {
                    user.Connection = conn;
                    return user;
                }
                else
                {
                    return null;
                }
            }
            var newUser = new User(username) { Connection = conn };
            DataBase.Add(username, newUser);
            newUser.OnDisconnect += (s, e) => newUser.Connection = null;
            return newUser;
        }
        return null;
    }

    public void Clear()
    {
        foreach (var user in DataBase.Values)
        {
            user.Connection = null;
        }
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

