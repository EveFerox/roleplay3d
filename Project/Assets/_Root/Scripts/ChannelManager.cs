﻿using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ChannelManager : Singleton<ChannelManager>
{
    readonly Dictionary<string, ChannelFeed> _channels = new Dictionary<string, ChannelFeed>();
    public static IReadOnlyDictionary<string, ChannelFeed> Channels => Instance._channels;

    ChannelFeed _globalFeed;

    protected void Awake()
    {
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
        if (string.IsNullOrWhiteSpace(name)) {
            Debug.LogWarning($"CreateChannel: invalid channel name");
            return null;
        }

        if (_channels.ContainsKey(name)) {
            Debug.LogWarning($"CreateChannel: {name} already exists");
            return null;
        }

        var channel = new ChannelFeed(name);
        channel.UserChanged += (sender, info) =>
        {
            switch (info.Change) {
                case ChannelFeed.UserChangeE.Unsubscribed:
                case ChannelFeed.UserChangeE.Disconnected:
                    if (info.Channel.Users.Count == 0) {
                        //Remove channel if everyone left it
                        RemoveChannel(info.Channel.Name);
                    }
                    break;
            }
        };

        _channels.Add(name, channel);

        return channel;
    }

    void RemoveChannel(string name)
    {
        _channels.Remove(name);
    }

    void HandleMessage(User user, ChatMessage msg)
    {
        if (msg.Message.Length == 0)
        {
            return;
        }
        if (_channels.TryGetValue(msg.Channel, out var channel))
        {
            if (msg.Message[0] == '/') {
                channel.Control(user, msg.Message.Substring(1));
            } else {
                channel.SendChat(user, msg);
            }
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

    public static void SubscribeToGlobal(User user)
    {
        Instance._globalFeed.Subscribe(user);
    }

    void Clear()
    {
        _channels.Clear();
        _globalFeed = null;
    }
}

