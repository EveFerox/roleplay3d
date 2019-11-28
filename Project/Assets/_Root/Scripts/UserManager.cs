using Mirror;
using System.Collections.Generic;

public class UserManager : Singleton<UserManager>
{
    readonly Dictionary<string, User> _users = new Dictionary<string, User>();
    public static IReadOnlyDictionary<string, User> Users => Instance._users;

    protected void Awake()
    {
        NetworkManager.StoppedHost += (sender, args) => {
            Clear();
        };

        NetworkManager.ClientDisconnected += (sender, conn) => {
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

    public static bool VerifyUser(string username, string password)
    {
        return Instance._users.TryGetValue(username, out var user) && user.Password.Verify(password);
    }

    public static User CreateUser(RegisterRequestMessage req)
    {
        if (Instance._users.ContainsKey(req.Username)) {
            return null;
        }

        var user = new User(req.Username, req.Password, req.Email);
        user.Disconnected += (s, e) => user.Connection = null;
        Instance._users.Add(req.Username, user);
        return user;
    }

    public static User Connect(NetworkConnection conn)
    {
        if (TryGetAuthUsername(conn, out var username) &&
            Instance._users.TryGetValue(username, out var user) &&
            user.Connection == null) {
            user.Connection = conn;
            return user;
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
        if (conn.isAuthenticated && conn.authenticationData is string un) {
            username = un;
            return true;
        }
        return false;
    }

    void Clear()
    {
        foreach (var user in _users.Values) {
            user.Connection = null;
        }
    }
}
