using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Extended <see cref="Mirror.NetworkManager"/>
/// </summary>
public class NetworkManager : Mirror.NetworkManager
{
    public static NetworkManager Instance => singleton as NetworkManager;

    SimpleAuthenticator _auth => authenticator as SimpleAuthenticator;

    public static event EventHandler StartedHost;
    public static event EventHandler StoppedHost;

    public static event EventHandler StartedClient;
    public static event EventHandler StoppedClient;

    public static event EventHandler StartedServer;
    public static event EventHandler StoppedServer;

    public static event EventHandler<NetworkConnection> ServerConnected;
    public static event EventHandler<NetworkConnection> ServerDisconnected;

    public static event EventHandler<NetworkConnection> ClientConnected;
    public static event EventHandler<NetworkConnection> ClientDisconnected;

    public static event EventHandler OnAuthSuccess { 
        add { Instance._auth.OnAuthSuccess += value; } 
        remove { Instance._auth.OnAuthSuccess -= value; } 
    }

    // 
    // public override void Awake()
    // {
    //     networkSceneName = offlineScene;
    //     InitializeSingleton();
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    // }

    public bool ValidateUsername(string username) => _auth.ValidateUsername(username);
    public bool ValidatePassword(string password) => _auth.ValidatePassword(password);
    public bool ValidateEmail(string email) => _auth.ValidateEmail(email);

    public void Login(string username, string password) {
        EnsureClientStarted();

        _auth.Login(username, password);
    }

    public void Register(RegisterInfo info) {
        EnsureClientStarted();

        _auth.Register(info);
    }

    void EnsureClientStarted() {
        if (NetworkClient.active) return;
        Debug.Log("Starting client");
        StartClient();
    }

    public override void OnStartHost() {
        base.OnStartHost();
        StartedHost?.Invoke(this, EventArgs.Empty);
    }

    public override void OnStopHost() {
        base.OnStopHost();
        StoppedHost?.Invoke(this, EventArgs.Empty);
    }

    public override void OnStartClient() {
        base.OnStartClient();
        StartedClient?.Invoke(this, EventArgs.Empty);
    }

    public override void OnStopClient() {
        base.OnStopClient();
        StoppedClient?.Invoke(this, EventArgs.Empty);
    }

    public override void OnStartServer() {
        base.OnStartServer();
        StartedServer?.Invoke(this, EventArgs.Empty);
    }

    public override void OnStopServer() {
        base.OnStopServer();
        StoppedServer?.Invoke(this, EventArgs.Empty);
    }

    public override void OnServerConnect(NetworkConnection conn) {
        base.OnServerConnect(conn);
        ServerConnected?.Invoke(this, conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn) {
        base.OnServerDisconnect(conn);
        ServerDisconnected?.Invoke(this, conn);
    }

    public override void OnClientConnect(NetworkConnection conn) {
        base.OnClientConnect(conn);
        ClientConnected?.Invoke(this, conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn) {
        base.OnClientDisconnect(conn);
        ClientDisconnected?.Invoke(this, conn);
    }

    // Transport.activeTransport maybe null, original code does not check this
    public override void OnApplicationQuit() {
        if (NetworkClient.isConnected) {
            StopClient();
            print("OnApplicationQuit: stopped client");
        }

        if (NetworkServer.active) {
            StopServer();
            print("OnApplicationQuit: stopped server");
        }

        Transport.activeTransport?.Shutdown();
    }
}
