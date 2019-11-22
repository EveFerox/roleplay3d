﻿using Mirror;
using System;

/// <summary>
/// Extended <see cref="Mirror.NetworkManager"/>
/// </summary>
public class NetworkManager : Mirror.NetworkManager
{
    public static NetworkManager Instance => singleton as NetworkManager;

    public static event EventHandler StartedHost;
    public static event EventHandler StopedHost;

    public static event EventHandler<NetworkConnection> ServerConnected;
    public static event EventHandler<NetworkConnection> ServerDisconnected;

    public static event EventHandler<NetworkConnection> ClientConnected;
    public static event EventHandler<NetworkConnection> ClientDisconnected;

    public override void OnStartHost()
    {
        base.OnStartHost();
        StartedHost?.Invoke(this, EventArgs.Empty);
    }
    public override void OnStopHost()
    {
        base.OnStopHost();
        StopedHost?.Invoke(this, EventArgs.Empty);
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
        base.OnClientDisconnect(conn);
        ClientDisconnected?.Invoke(this, conn);
    }

    // Transport.activeTransport maybe null, original code does not check this
    public override void OnApplicationQuit()
    {
        if (NetworkClient.isConnected)
        {
            StopClient();
            print("OnApplicationQuit: stopped client");
        }

        if (NetworkServer.active)
        {
            StopServer();
            print("OnApplicationQuit: stopped server");
        }

        Transport.activeTransport?.Shutdown();
    }
}
