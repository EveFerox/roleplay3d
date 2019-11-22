using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour
    where T : Singleton<T>
{
    static bool _shuttingDown = false;
    static readonly object _lock = new object();
    static T _instance;

    public static T Instance
    {
        get
        {
            if (_shuttingDown) {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed. Returning null.");
                return null;
            }

            lock (_lock) {
                if (_instance == null) {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null) {
                        var singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).Name + " (Singleton)";
                    }

                    DontDestroyOnLoad(_instance);
                }
                return _instance;
            }
        }
    }

    void OnApplicationQuit()
    {
        _shuttingDown = true;
    }

    void OnDestroy()
    {
        _shuttingDown = true;
    }
}
