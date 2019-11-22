using UnityEngine;
using System.Collections;

public class ChangedEventArgs<T>
{
    public T OldValue { get; }
    public T NewValue { get; }

    public ChangedEventArgs(T oldValue, T newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
