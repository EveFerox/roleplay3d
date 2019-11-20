using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ChannelManager : StaticMonoBehaviour<ChannelManager>
{
    readonly Dictionary<string, ChannelFeed> Channels = new Dictionary<string, ChannelFeed>();

    ChannelFeed _globalFeed;

    protected override void Awake()
    {
        base.Awake();

        NetworkManager.StartedHost += (sender, args) =>
        {
            _globalFeed = CreateChannel("Global");

            NetworkServer.RegisterHandler((NetworkConnection conn, ChatMessage msg) =>
            {
                HandleMessage(UserManager.GetUser(conn), msg);
            });
        };
        NetworkManager.StopedHost += (sender, args) =>
        {
            Clear();
        };
    }

    public ChannelFeed CreateChannel(string name)
    {
        if (Channels.ContainsKey(name)) {
            Debug.Log($"Channel {name} already exists");
            return null;
        }

        var channel = new ChannelFeed {Name = name};
        Channels.Add(name, channel);

        return channel;
    }

    void HandleMessage(User user, ChatMessage msg)
    {
        if (msg.Message.Length == 0)
        {
            return;
        }
        if (Channels.TryGetValue(msg.Channel, out var channel))
        {
            if (msg.Message[0] == '/')
            {
                channel.Control(user, msg.Message.Substring(1));
            }
            channel.SendChat(user, msg);
        }
        else if (msg.Message.IndexOf("/create ") == 0)
        {
            var c = CreateChannel(msg.Message.Substring(8));
            if (c != null)
            {
                c.Subscribe(user);
            }
        }
        else if (msg.Message.IndexOf("/logoff") == 0)
        {
            user.Connection.Disconnect();
        }
    }

    public void SubscribeToGlobal(User user)
    {
        _globalFeed.Subscribe(user);
    }

    public void Clear()
    {
        Channels.Clear();
        _globalFeed = null;
    }
}

