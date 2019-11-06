using System;
using UnityEngine;
using System.Collections;
using Mirror;
using Newtonsoft.Json;

public struct ChatMessage : IMessageBase
{
    public string Sender { get; set; }
    public string Message { get; set; }
    public DateTime Time { get; set; }

    public string AsString()
    {
        return $"[{Time.ToShortTimeString()}] {Sender}: {Message}";
    }

    public void Copy(ChatMessage other)
    {
        Sender = other.Sender;
        Message = other.Message;
        Time = other.Time;
    }
    
    public void Deserialize(NetworkReader reader)
    {
        var str = reader.ReadString();
        var other = JsonConvert.DeserializeObject<ChatMessage>(str);
        Copy(other);
    }
    public void Serialize(NetworkWriter writer)
    {
        var json = JsonConvert.SerializeObject(this);
        writer.WriteString(json);
    }
}
