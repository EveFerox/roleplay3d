﻿using System.Collections.Generic;
using UnityEngine;

public class ChannelManager : MonoBehaviour
{
    private readonly Dictionary<string, ChannelFeed> Channels = new Dictionary<string, ChannelFeed>();

    private ChannelFeed globalFeed;

    private void Awake()
    {
        globalFeed = CreateChannel("Global");
    }

    public ChannelFeed CreateChannel(string name)
    {
        if (Channels.ContainsKey(name))
        {
            return null;
        }

        var channel = gameObject.AddComponent<ChannelFeed>();
        channel.Name = name;
        Channels.Add(name, channel);

        return channel;
    }

    public void HandleMessage(User user, ChatMessage msg)
    {
        if (msg.Message.Length == 0) return; 
        if (Channels.TryGetValue(msg.Channel, out var channel))
        {
            if (msg.Message[0] == '/') channel.Control(user, msg.Message.Substring(1));
            channel.SendChat(user, msg);
        }
        else if (msg.Message.IndexOf("/create ") == 0)
        {
            var c = CreateChannel(msg.Message.Substring(8));
            c.Subscribe(user);
        }
    }

    public void SubscribeToGlobal(User user)
    {
        globalFeed.Subscribe(user);
    }
}
