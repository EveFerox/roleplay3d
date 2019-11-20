using System;
using Mirror;

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