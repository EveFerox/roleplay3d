using System;
using UnityEngine;
using System.Collections;
using Mirror;
using Newtonsoft.Json;

public class ChatMessage : DefaultMessageBase
{
    public string Sender { get; set; }
    public string Channel { get; set; }
    public string Message { get; set; }
    public DateTime Time { get; set; }

    public string AsString()
    {
        return $"[{Time.ToShortTimeString()} - {Channel}] {Sender}: {Message}";
    }

    protected override void CopyFrom(object obj)
    {
        if (obj is ChatMessage v)
        {
            Sender = v.Sender;
            Channel = v.Channel;
            Message = v.Message;
            Time = v.Time;
        }
    }
}
