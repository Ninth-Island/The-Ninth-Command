using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class VirtualPlayer : NetworkBehaviour{

    [SerializeField] private GameObject gamePlayerPrefab;
    private LobbyPlayer _lobbyPlayer;
    private Player _gamePlayer;


    public void SetLobbyPlayer(LobbyPlayer lobbyPlayer){
        _lobbyPlayer = lobbyPlayer;
    }

    public LobbyPlayer GetLobbyPlayer(){
        return _lobbyPlayer;
    }


    [Server]
    public void SetupPlayer(Player player, string username, Color[] colors){
        ClientSetupPlayer(player, username, colors);

    }

    [ClientRpc]
    private void ClientSetupPlayer(Player player, string username, Color[] colors){
        _gamePlayer = player;
        gameObject.name = username;
        
        Transform sprites = _gamePlayer.transform.GetChild(1);
        sprites.GetChild(0).GetComponent<SpriteRenderer>().color = colors[3]; // body
        
        sprites.GetChild(1).GetComponent<SpriteRenderer>().color = colors[2]; // arms unarmed
        sprites.GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().color = colors[2]; // arms armed
        
        sprites.GetChild(2).GetChild(0).GetComponent<SpriteRenderer>().color = colors[1]; // helmet
        sprites.GetChild(2).GetChild(1).GetComponent<SpriteRenderer>().color = colors[0]; // visor
    }





}
