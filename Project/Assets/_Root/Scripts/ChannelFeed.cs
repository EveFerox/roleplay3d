using Mirror;
using System;
using System.Collections.Generic;

public class ChannelFeed
{
    public event EventHandler<UserChangeInfo> UserChanged;

    public event EventHandler Removed;

    readonly HashSet<User> Users = new HashSet<User>();
    readonly List<ChatMessage> ChatLog = new List<ChatMessage>();

    public string Name { get; set; }
    public bool SaveMessages { get; set; } = true;

    protected virtual bool CanSubscribe(User user)
    {
        return !IsSubscribed(user);
    }

    protected virtual void OnSubscribe(User user)
    {
        Users.Add(user);
        user.OnDisconnect += (s, e) => { OnDisconnect(user); };
        UserChanged?.Invoke(this, new UserChangeInfo(this, user, UserChangeE.Subscribed));
        SendInfo($"{user.Username} connected");
    }

    protected virtual void OnDisconnect(User user)
    {
        Users.Remove(user);
        UserChanged?.Invoke(this, new UserChangeInfo(this, user, UserChangeE.Disconnected));
        SendInfo($"{user.Username} disconnected");
    }

    protected virtual void OnUnsubscribe(User user)
    {
        Users.Remove(user);
        UserChanged?.Invoke(this, new UserChangeInfo(this, user, UserChangeE.Unsubscribed));
        SendInfo($"{user.Username} left");
    }

    protected virtual void OnRemoveChannel()
    {
        SendInfo($"channel '{Name}' removed");
        Removed?.Invoke(this, EventArgs.Empty);
    }

    protected virtual bool CanSendMessage(User user)
    {
        return IsSubscribed(user);
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
        if (CanSubscribe(user) == false) return;

        OnSubscribe(user);
    }

    public void Unsubscribe(User user)
    {
        OnUnsubscribe(user);
    }

    public void RemoveChannel()
    {
        OnRemoveChannel();
    }

    protected bool SendToAll<T>(T msg, int channelId = Channels.DefaultReliable) 
        where T : IMessageBase
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

    public enum UserChangeE
    {
        Subscribed,
        Unsubscribed,
        Disconnected
    }

    public class UserChangeInfo
    {
        public ChannelFeed Channel { get; }
        public User User { get; }
        public UserChangeE Change { get; }

        public UserChangeInfo(ChannelFeed channel, User user, UserChangeE change)
        {
            Channel = channel;
            User = user;
            Change = change;
        }
    }
}