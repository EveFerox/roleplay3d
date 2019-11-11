﻿using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    public event EventHandler<bool> CanSendChange;
    public event EventHandler<ChatMessage> MessageReceived;

    bool _canSend;
    public bool CanSend
    {
        get => _canSend;
        set
        {
            if (_canSend == value) return;
            _canSend = value; 
            CanSendChange?.Invoke(this, value);
        }
    }

    readonly List<ChatMessage> _messages = new List<ChatMessage>();
    public IReadOnlyList<ChatMessage> Messages => _messages;

    void Awake()
    {
        NetworkManager.StartedHost += (sender, args) =>
        {
            Debug.Log($"StartedHost");

            NetworkServer.RegisterHandler((NetworkConnection conn, ChatMessage msg) =>
            {
                Debug.Log($"Server Received '{msg.Message}' from {conn.address}");

                msg.Sender = conn.address;
                msg.Time = DateTime.UtcNow;
                NetworkServer.SendToAll(msg);
            });
        };
        NetworkManager.StopedHost += (sender, args) =>
        {
            Debug.Log($"StopedHost");

            CanSend = false;
        };

        NetworkManager.ServerConnected += (sender, connection) =>
        {
            Debug.Log($"ServerConnected {connection.address}");

            SendAsSever($"{connection.address} connected");
        };
        NetworkManager.ServerDisconnected += (sender, connection) =>
        {
            Debug.Log($"ServerDisconnected {connection.address}");

            SendAsSever($"{connection.address} disconnected");
        };

        NetworkManager.ClientConnected += (sender, connection) =>
        {
            Debug.Log($"ClientConnected {connection.address}");

            _messages.Clear();
            CanSend = true;

            NetworkClient.RegisterHandler((NetworkConnection conn, ChatMessage msg) =>
            {
                Debug.Log($"Client Received '{msg.Message}' from {msg.Sender}");

                MessageReceived?.Invoke(this, msg);

                _messages.Add(msg);
            });
        };
        NetworkManager.ClientDisconnected += (sender, connection) =>
        {
            Debug.Log($"ClientDisconnected {connection.address}");

            CanSend = false;
        };
    }

    public void Send(string text)
    {
        NetworkClient.Send(new ChatMessage { Message = text });
    }

    public void SendAsSever(string text)
    {
        if (NetworkServer.active == false) {
            Debug.LogError("NetworkServer.active==false", this);
            return;
        }

        var msg = new ChatMessage
        {
            Message = text, 
            Sender = "SERVER",
            Time = DateTime.UtcNow
        };
        NetworkServer.SendToAll(msg);
    }
}
