using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;


public class ChannelFeed : MonoBehaviour
{
    private readonly HashSet<User> Users = new HashSet<User>();
    private readonly List<ChatMessage> ChatLog = new List<ChatMessage>();

    public event EventHandler Removed;

    public string Name { get; set; }
    public bool SaveMessages { get; set; } = true;

    public float RemoveTimer { get; set; } = 3600;


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
        if (Users.Count == 0) Invoke(nameof(RemoveChannelSelf), RemoveTimer);
    }

    protected virtual void OnUnsubscribe(User user)
    {
        SendInfo($"{user.Username} left");
        Users.Remove(user);
        if (Users.Count == 0) Invoke(nameof(RemoveChannelSelf), RemoveTimer);
    }

    protected virtual void OnRemoveChannel()
    {
        SendInfo($"channel removed");
        Removed?.Invoke(this, EventArgs.Empty);
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
            CancelInvoke(nameof(RemoveChannelSelf));
        }
    }

    public void Unsubscribe(User user)
    {
        OnUnsubscribe(user);
    }

    public void RemoveChannel()
    {
        OnRemoveChannel();
    }

    private void RemoveChannelSelf()
    {
        if (Users.Count == 0) OnRemoveChannel();
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