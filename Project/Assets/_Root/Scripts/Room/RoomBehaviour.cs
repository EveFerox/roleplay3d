using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class RoomBehaviour : NetworkBehaviour
{
    protected bool hidden;

    NetworkRoom Room;

    public NetworkIdentity Identity { get; private set; }
    public NetworkConnection Connection => Identity.connectionToClient;

    protected virtual void Awake()
    {
        Identity = GetComponent<NetworkIdentity>();
    }

    public override bool OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize)
    {
        if (Room == null) {
            observers.Clear();
            return true;
        }
        observers.RemoveWhere(conn => !Room.Contains(conn));
        foreach(var conn in Room.Elements) {
            observers.Add(Connection);
        }
        return true;
    }

    public override bool OnCheckObserver(NetworkConnection newObserver)
    {
        if (Room == null) {
            return false;
        }
        if (newObserver.identity.GetComponentInParent<RoomBehaviour>() is RoomBehaviour element) {
            return Room.Contains(element.Connection);
        }
        return false;
    }

    public override void OnSetLocalVisibility(bool visible)
    {
        foreach (var rend in GetComponentsInChildren<Renderer>()) {
            rend.enabled = visible;
        }
    }
}
