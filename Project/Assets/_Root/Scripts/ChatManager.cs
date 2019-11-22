using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>
{
    protected ChatManager() { }

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

    protected void Awake()
    {
        NetworkManager.StopedHost += (sender, args) =>
        {
            CanSend = false;
        };

        NetworkManager.ClientConnected += (sender, connection) =>
        {
            _messages.Clear();
            CanSend = true;

            NetworkClient.RegisterHandler((NetworkConnection conn, ChatMessage msg) =>
            {
                MessageReceived?.Invoke(this, msg);
            
                _messages.Add(msg);
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
