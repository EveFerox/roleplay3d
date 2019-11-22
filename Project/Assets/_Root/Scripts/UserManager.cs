using Mirror;
using System.Collections.Generic;

public class UserManager : Singleton<UserManager>
{
    protected UserManager() { }
    readonly Dictionary<string, User> _users = new Dictionary<string, User>();
    public static IReadOnlyDictionary<string, User> Users => Instance._users;

    protected void Awake()
    {       
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
        if (TryGetAuthUsername(conn, out var username) && Instance._users.TryGetValue(username, out var user)) {
            return user;
        }
        return null;
    }

    public static User Register(NetworkConnection conn)
    {
        if (TryGetAuthUsername(conn, out var username)) {
            if (Instance._users.TryGetValue(username, out var user)) {
                if (user.Connection == null) {
                    user.Connection = conn;
                    return user;
                } else {
                    return null;
                }
            }
            var newUser = new User(username) {Connection = conn};
            Instance.AddUser(newUser);
            return newUser;
        }
        return null;
    }

    void AddUser(User user)
    {
        _users.Add(user.Username, user);
        user.Disconnected += (s, e) => user.Connection = null;
    }

    static bool TryGetAuthUsername(NetworkConnection conn, out string username)
    {
        username = null;
        if (conn.isAuthenticated && conn.authenticationData is string un)
            return true;
        return false;
    }

    void Clear()
    {
        foreach (var user in _users.Values) {
            user.Connection = null;
        }
    }
}