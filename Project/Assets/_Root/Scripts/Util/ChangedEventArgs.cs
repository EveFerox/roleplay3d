using UnityEngine;
using System.Collections;

public class ChangedEventArgs<T>
{
    public T OldValue { get; private set; }
    public T NewValue { get; private set; }

    public ChangedEventArgs(T oldValue, T newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
