using System;
using UnityEngine;
using System.Collections;
using Mirror;

public struct ChatMessage
{
    public NetworkConnection Sender { get; set; }
    public string Message { get; set; }
    public DateTime Time { get; set; }
}
