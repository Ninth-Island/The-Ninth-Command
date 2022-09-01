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
        
        // if the game is running, spawn an actual character
        else if (sceneName != "Assets/Scenes/Lobby.unity" && sceneName != "Assets/Scenes/Menu.unity"){
            SetupPlayer(conn);
        }
    }


    [Server]
    private void SetupPlayer(NetworkConnectionToClient connectionToClient){
        
        Player player = Instantiate(gamePlayerPrefab).GetComponent<Player>();
        BasicWeapon pW = Instantiate(player.primaryWeaponPrefab);
        BasicWeapon sW = Instantiate(player.secondaryWeaponPrefab);
        NetworkServer.Spawn(player.gameObject, connectionToClient);

        pW.PickUp(player, player.arm);
                
        sW.PickUp(player, player.arm);
        sW.activelyWielded = false;
        sW.gameObject.SetActive(false);

        player.primaryWeapon = pW;
        player.secondaryWeapon = sW;
                
        NetworkServer.Spawn(pW.gameObject, connectionToClient);
        NetworkServer.Spawn(sW.gameObject, connectionToClient);
    }

}
