using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkManager : Mirror.NetworkManager
{
    public static event EventHandler<NetworkConnection> ServerConnected;
    public static event EventHandler<NetworkConnection> ServerDisconnected;

    public static event EventHandler<NetworkConnection> ClientConnected;
    public static event EventHandler<NetworkConnection> ClientDisconnected;
    
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
}
