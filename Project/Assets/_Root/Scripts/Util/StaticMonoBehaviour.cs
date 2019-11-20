using UnityEngine;
using System.Collections;

public abstract class StaticMonoBehaviour<T> : MonoBehaviour 
    where T : StaticMonoBehaviour<T>
{
    protected static T Instance { get; private set; }

    protected virtual void Awake()
    {
        Instance = this as T;
    }
    protected virtual void Start()
    {

    }
}
