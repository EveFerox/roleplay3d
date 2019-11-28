using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRoom : MonoBehaviour
{

    HashSet<RoomBehaviour> _elements = new HashSet<RoomBehaviour>();
    private ChannelFeed _channel;


    public IReadOnlyCollection<RoomBehaviour> Elements => _elements;


    public string Name => _channel.Name;


    [Scene]
    public string RoomScene;

    public Transform startPosition;

    public GameObject playerPrefab;
  

    void LoadScaneForPlayer(NetworkConnection conn, GameObject roomPlayer)
    {
        if (SceneManager.GetActiveScene().name == RoomScene) {
            return;
        }

        var gamePlayer = startPosition != null
            ? Instantiate(playerPrefab, startPosition.position, startPosition.rotation)
            : Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        gamePlayer.name = playerPrefab.name;

        NetworkServer.ReplacePlayerForConnection(conn, gamePlayer);
    }


    public bool Contains(NetworkConnection conn)
    {
        return Elements.Select(e => e.Connection).Contains(conn);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Additive) {
            if (NetworkServer.active) {
                // TODO only respawn the server objects from that scene later!
                // NetworkServer.SpawnObjects();
                Debug.Log("Respawned Server objects after additive scene load: " + scene.name);
            }
            if (NetworkClient.active) {
                ClientScene.PrepareToSpawnSceneObjects();
                Debug.Log("Rebuild Client spawnableObjects after additive scene load: " + scene.name);
            }
        }
    }
}
