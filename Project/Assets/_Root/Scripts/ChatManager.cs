using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>
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

    protected void Awake()
    {
        NetworkManager.StopedHost += (sender, args) =>
        {
            CanSend = false;
        };

        NetworkManager.ClientConnected += (sender, connection) =>
        {
            CanSend = true;

            NetworkClient.RegisterHandler((NetworkConnection conn, ChatMessage msg) =>
            {
                MessageReceived?.Invoke(this, msg);
            });
        };
        NetworkManager.ClientDisconnected += (sender, connection) =>
        {
            CanSend = false;
        };
    }

    public void Send(string text, string channel)
    {
        NetworkClient.Send(new ChatMessage { Message = text, Channel = channel });
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
