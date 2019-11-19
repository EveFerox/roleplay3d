using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;


public class ChannelFeed : MonoBehaviour
{
    private readonly HashSet<User> Users = new HashSet<User>();
    private readonly List<ChatMessage> ChatLog = new List<ChatMessage>();

    public string Name { get; set; }
    public bool SaveMessages { get; set; } = true;

    protected virtual bool CanSubscribe(User conn)
    {
        return !IsSubscribed(conn);
    }

    protected virtual void OnSubscribe(User user)
    {
        SendInfo($"{user.Username} connected");
    }

    protected virtual void OnDisconnect(User user)
    {
        Users.Remove(user);
        SendInfo($"{user.Username} disconnected");
    }

    protected virtual void OnUnsubscribe(User user)
    {
        SendInfo($"{user.Username} left");
        Users.Remove(user);
    }

    protected virtual bool CanSendMessage(User user)
    {
        return true;
    }

    public virtual void Control(User user, string message)
    {
        switch (message)
        {
            case "leave":
                Unsubscribe(user);
                break;
            case "join":
                if (CanSubscribe(user))
                {
                    Subscribe(user);
                }

                break;
        }
    }

    public bool IsSubscribed(User user)
    {
        return Users.Contains(user);
    }

    public void Subscribe(User user)
    {
        if (CanSubscribe(user))
        {
            Users.Add(user);
            OnSubscribe(user);
            user.OnDisconnect += (s, e) => { OnDisconnect(user); };
        }
    }

    public void Unsubscribe(User user)
    {
        OnUnsubscribe(user);
    }

    protected bool SendToAll<T>(T msg, int channelId = Channels.DefaultReliable) where T : IMessageBase
    {
        var result = true;
        if (SaveMessages && msg is ChatMessage chat)
        {
            ChatLog.Add(chat);
        }
        foreach (var user in Users)
        {
            result &= user.Connection.Send(msg, channelId);
        }
        return result;
    }

    public void SendChat(User user, ChatMessage msg)
    {
        if (!CanSendMessage(user)) { return; }
        msg.Sender = user.Username;
        msg.Channel = Name;
        msg.Time = DateTime.UtcNow;
        SendToAll(msg);
        if (!Users.Contains(user))
        {
            msg.Sender = "#";
            user.Connection.Send(msg);
        }
    }

    protected void SendInfo(string msg)
    {
        SendToAll(new ChatMessage
        {
            Sender = "@",
            Channel = Name,
            Time = DateTime.UtcNow,
            Message = msg
        });
    }
}