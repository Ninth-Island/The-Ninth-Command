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
        string sceneName = SceneManager.GetActiveScene().name;
        
        // if in the lobby, spawn a lobby character
        if (sceneName == "Lobby"){
            NetworkServer.Spawn(Instantiate(lobbyPlayerPrefab, new Vector3(10000, 10000, 0), Quaternion.identity, FindObjectOfType<Canvas>().transform), conn);
        }
    }

    public override void OnServerSceneChanged(string sceneName){
        if (sceneName != "Assets/Scenes/Lobby.unity" && sceneName != "Assets/Scenes/Menu.unity"){
            foreach (NetworkConnectionToClient connectionToClient in NetworkServer.connections.Values){
                SetupPlayer(connectionToClient);
            }
        }}


    [Server]
    private void SetupPlayer(NetworkConnectionToClient connectionToClient){
        
        Player player = Instantiate(gamePlayerPrefab).GetComponent<Player>();
        BasicWeapon pW = Instantiate(player.primaryWeaponPrefab);
        BasicWeapon sW = Instantiate(player.secondaryWeaponPrefab);

        NetworkServer.Spawn(player.gameObject, connectionToClient);
        NetworkServer.Spawn(pW.gameObject, connectionToClient);
        NetworkServer.Spawn(sW.gameObject, connectionToClient);


        player.primaryWeapon = pW;
        player.secondaryWeapon = sW;
        pW.StartCoroutine(pW.ServerInitializeWeapon(true, player));
        sW.StartCoroutine(sW.ServerInitializeWeapon(false, player));
    }

}
