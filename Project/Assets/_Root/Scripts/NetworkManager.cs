using Mirror;
using System;
using UnityEngine;

[RequireComponent(typeof(UserManager)),
 RequireComponent(typeof(ChannelManager))]
public class NetworkManager : Mirror.NetworkManager
{
    public static event EventHandler StartedHost;
    public static event EventHandler StopedHost;

    public static event EventHandler<NetworkConnection> ServerConnected;
    public static event EventHandler<NetworkConnection> ServerDisconnected;

    public static event EventHandler<NetworkConnection> ClientConnected;
    public static event EventHandler<NetworkConnection> ClientDisconnected;

    private UserManager users;
    private ChannelManager channels; 

    public override void Awake()
    {
        base.Awake();
        users = GetComponent<UserManager>();
        channels = GetComponent<ChannelManager>();
    }

    public override void OnStartHost()
    {
        base.OnStartHost();

        channels.OnServerStarted();

        NetworkServer.RegisterHandler((NetworkConnection conn, ChatMessage msg) =>
        {
            channels.HandleMessage(users.GetUser(conn), msg);
        });

        StartedHost?.Invoke(this, EventArgs.Empty);
    }
    public override void OnStopHost()
    {
        base.OnStopHost();
        StopedHost?.Invoke(this, EventArgs.Empty);
        users.Clear();
        channels.Clear();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        ServerConnected?.Invoke(this, conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        ServerDisconnected?.Invoke(this, conn);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        ClientConnected?.Invoke(this, conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        users.GetUser(conn)?.DisconnectedInternal();
        base.OnClientDisconnect(conn);
        ClientDisconnected?.Invoke(this, conn);
    }
}
