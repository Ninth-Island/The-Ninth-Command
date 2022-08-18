using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager{

    [SerializeField] private GameObject lobbyPlayerPrefab;
    public override void OnServerAddPlayer(NetworkConnectionToClient conn){
        base.OnServerAddPlayer(conn);
        
        NetworkServer.Spawn(Instantiate(lobbyPlayerPrefab, new Vector3(10000, 10000, 0), Quaternion.identity, FindObjectOfType<Canvas>().transform), conn);
    }
}