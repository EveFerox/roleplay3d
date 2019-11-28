using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRoomsManager : Mirror.NetworkManager
{
    public override void Awake()
    {
        networkSceneName = offlineScene;

        Initialize();

        SceneManager.sceneLoaded += SceneLoaded;
    }

    public override void ServerChangeScene(string newSceneName, LoadSceneMode sceneMode, LocalPhysicsMode physicsMode) { 
    }

    void Initialize()
    {
        if (singleton != null && singleton == this) {
            return;
        }
        LogFilter.Debug = showDebugMessages;
        if (dontDestroyOnLoad) {
            if (singleton != null) {
                Debug.LogWarning("Multiple NetworkManagers detected in the scene. Only one NetworkManager can exist at a time. The duplicate NetworkManager will be destroyed.");
                Destroy(gameObject);
                return;
            }
            if (LogFilter.Debug) Debug.Log("NetworkManager created singleton (DontDestroyOnLoad)");
            singleton = this;
            if (Application.isPlaying) DontDestroyOnLoad(gameObject);
        } else {
            if (LogFilter.Debug) Debug.Log("NetworkManager created singleton (ForScene)");
            singleton = this;
        }
        Transport.activeTransport = transport;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Additive) {
            if (NetworkClient.active) {
                ClientScene.PrepareToSpawnSceneObjects();
                Debug.Log("Rebuild Client spawnableObjects after additive scene load: " + scene.name);
            }
        }
    }


    #region NetworkServer

    bool networkServerLocalClientActive;

    void NetworkServer_ActivateLocalClientScene()
    {
        if (networkServerLocalClientActive) return;
        networkServerLocalClientActive = true;
        foreach (NetworkIdentity identity in NetworkIdentity.spawned.Values) {
            if (!identity.isClient) {
                if (LogFilter.Debug) Debug.Log("ActivateClientScene " + identity.netId + " " + identity);

                // identity.OnStartClient();
            }
        }
    }

    #endregion


    public static object CallStaticPrivate(Type type, string name, params object[] parameters)
    {
        return type.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, parameters);
    }

    public static object CallStaticPrivate<T>(string name, params object[] parameters)
    {
        return typeof(T).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, parameters);
    }

    public static object CallPrivate<T>(T obj, string name, params object[] parameters)
    {
        return typeof(T).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(obj, parameters);
    }

}
