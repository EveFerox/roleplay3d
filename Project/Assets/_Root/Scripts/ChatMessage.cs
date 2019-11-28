using System;
using UnityEngine;
using System.Collections;
using Mirror;
using Newtonsoft.Json;

public class ChatMessage : IMessageBase
{
    public string Sender { get; set; }
    public string Channel { get; set; }
    public string Message { get; set; }
    public DateTime Time { get; set; }

    public string AsString()
    {
        return $"[{Time.ToShortTimeString()} - {Channel}] {Sender}: {Message}";
    }

    public void Deserialize(NetworkReader reader)
    {
        Time = DateTime.FromBinary(reader.ReadInt64());
        Sender = reader.ReadString();
        Channel = reader.ReadString();
        Message = reader.ReadString();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.WriteInt64(Time.ToBinary());
        writer.WriteString(Sender);
        writer.WriteString(Channel);
        writer.WriteString(Message);
    }
}
