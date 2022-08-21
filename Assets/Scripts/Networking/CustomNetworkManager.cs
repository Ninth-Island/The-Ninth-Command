using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager{

    [SerializeField] private GameObject lobbyPlayerPrefab;
    [SerializeField] private GameObject gamePlayerPrefab;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn){
        base.OnServerAddPlayer(conn);
        if (SceneManager.GetActiveScene().name == "Lobby"){
            NetworkServer.Spawn(
                Instantiate(lobbyPlayerPrefab, new Vector3(10000, 10000, 0), Quaternion.identity,
                    FindObjectOfType<Canvas>().transform), conn);
        }
    }

    public override void OnServerSceneChanged(string sceneName){
        if (sceneName != "Assets/Scenes/Lobby.unity" && sceneName != "Assets/Scenes/Menu.unity"){
            foreach (KeyValuePair<int, NetworkConnectionToClient> connectionToClient in NetworkServer.connections){
                NetworkServer.Spawn(Instantiate(gamePlayerPrefab), connectionToClient.Value);
            }
        }
        
    }
}
