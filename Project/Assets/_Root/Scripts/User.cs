using System;
using Mirror;

public class User
{
    public event EventHandler Connected;
    public event EventHandler Disconnected;

    public readonly string Username;
    public readonly PasswordHash Password;
    public readonly string Email;

    public NetworkConnection Connection { get; set; }

    public bool IsConnected => Connection?.isAuthenticated ?? false;

    internal void DisconnectedInternal()
    {
        Disconnected?.Invoke(this, EventArgs.Empty);
    }

    public User(string username, string password, string email)
    {
        Username = username;
        Password = new PasswordHash(password);
        Email = email;
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