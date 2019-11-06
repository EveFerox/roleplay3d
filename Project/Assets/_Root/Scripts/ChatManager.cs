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

    void Start()
    { 
        NetworkServer.RegisterHandler((NetworkConnection conn, StringMessage msg) =>
        {
            Debug.Log($"Server Received '{msg.value}' from {conn.address}");
            NetworkServer.SendToAll(msg);
        });

        NetworkClient.RegisterHandler((NetworkConnection conn, StringMessage msg) =>
        {
            Debug.Log($"Client Received '{msg.value}' from {conn.address}");

            _messages.Add(new ChatMessage{Sender = conn, Message = msg.value, Time = DateTime.Now});

            //Force scroll to bottom
            _scroll += Vector2.down * -500;
        });
    }

    //[Command]
    //void CmdClientSend(string msg)
    //{
    //    Debug.Log($"ClientSend Received '{msg}");
    //}

    void OnGUI()
    {
        _windowChatRect = GUI.Window(500, _windowChatRect, WindowChatFunc, "Chat");
    }

    void WindowChatFunc(int id)
    {
        GUI.DragWindow(new Rect(0, 0, 10000, 20));

        GUILayout.Label("Message:");
        _message = GUILayout.TextArea(_message, 500);
        
        if (GUILayout.Button("Send")) {
            NetworkClient.Send(new StringMessage(_message));
        }
        if (GUILayout.Button("Send 2")) {
            //CmdClientSend(_message);
        }

        _scroll = GUILayout.BeginScrollView(_scroll);

        foreach (var m in _messages) {
            GUILayout.Label($"[{m.Time.ToShortTimeString()}] {m.Sender.address}: {m.Message}");
        }

        GUILayout.EndScrollView();
    }
}
