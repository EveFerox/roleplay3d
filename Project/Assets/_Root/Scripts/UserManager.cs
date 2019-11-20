using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class UserManager : StaticMonoBehaviour<UserManager>
{
    readonly Dictionary<string, User> DataBase = new Dictionary<string, User>();

    protected override void Awake()
    {
        base.Awake();
        
        NetworkManager.StopedHost += (sender, args) =>
        {
            Clear();
        };

        NetworkManager.ClientDisconnected += (sender, conn) =>
        {
            GetUser(conn)?.DisconnectedInternal();
        };
    }

    public static User GetUser(NetworkConnection conn)
    {
        if (conn.isAuthenticated && conn.authenticationData is string username && Instance.DataBase.TryGetValue(username, out var user)) {
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