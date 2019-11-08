using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    [SerializeField]
    string _message = "MESSAGE!!!";

    [SerializeField]
    Rect _windowChatRect = new Rect(250, 250, 500, 300);

    readonly List<ChatMessage> _messages = new List<ChatMessage>();

    Vector2 _scroll;

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
        };

        NetworkManager.ServerConnected += (sender, connection) =>
        {
            Debug.Log($"ServerConnected {connection.address}");
        };
        NetworkManager.ServerDisconnected += (sender, connection) =>
        {
            Debug.Log($"ServerDisconnected {connection.address}");
        };

        NetworkManager.ClientConnected += (sender, connection) =>
        {
            Debug.Log($"ClientConnected {connection.address}");

            _messages.Clear();

            NetworkClient.RegisterHandler((NetworkConnection conn, ChatMessage msg) =>
            {
                Debug.Log($"Client Received '{msg.Message}' from {msg.Sender}");

                _messages.Add(msg);

                //Force scroll to bottom
                _scroll += Vector2.down * -500;
            });
        };
        NetworkManager.ClientDisconnected += (sender, connection) =>
        {
            Debug.Log($"ClientDisconnected {connection.address}");
        };
    }

    void OnGUI()
    {
        if (NetworkClient.isConnected)
            _windowChatRect = GUI.Window(500, _windowChatRect, WindowChatFunc, "Chat");
    }

    void WindowChatFunc(int id)
    {
        GUI.DragWindow(new Rect(0, 0, 10000, 20));

        GUILayout.Label("Message:");
        _message = GUILayout.TextArea(_message, 500);
        
        if (GUILayout.Button("Send")) {
            NetworkClient.Send(new ChatMessage { Message = _message });
        }

        if (NetworkServer.active && GUILayout.Button("Send As Server")) {
            var msg = new ChatMessage
            {
                Message = _message, 
                Sender = "SERVER",
                Time = DateTime.UtcNow
            };
            NetworkServer.SendToAll(msg);
        }

        _scroll = GUILayout.BeginScrollView(_scroll);

        foreach (var m in _messages) {
            GUILayout.Label(m.AsString());
        }

        GUILayout.EndScrollView();
    }
}
