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

    private string _username;

    private Color[] _colors = new[]{new Color(1, 0.5716f, 0, 1), new Color(0, 0.7961729f, 1, 1), new Color(0, 0.7961729f, 1, 1), new Color(0, 0.7961729f, 1, 1)};

    
    public override void OnStartServer(){
        DontDestroyOnLoad(gameObject);
    }

    public void SetLobbyPlayer(LobbyPlayer lobbyPlayer){
        _lobbyPlayer = lobbyPlayer;
    }

    public LobbyPlayer GetLobbyPlayer(){
        return _lobbyPlayer;
    }

    public void SetUsername(string username){
        _username = username;
    }

    public void SetColors(Color[] colors){
        _colors = colors;
    }


    [Server]
    public void SetupPlayer(Player player){
        ClientSetupPlayer(player);
    }

    [ClientRpc]
    private void ClientSetupPlayer(Player player){
        Debug.Log("client");
        _gamePlayer = player;
        _gamePlayer.name = _username;
        Color[] colors = _colors;
        
        Transform sprites = _gamePlayer.transform.GetChild(1);
        sprites.GetChild(0).GetComponent<SpriteRenderer>().color = colors[3]; // body
        
        sprites.GetChild(1).GetComponent<SpriteRenderer>().color = colors[2]; // arms unarmed
        sprites.GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().color = colors[2]; // arms armed
        
        sprites.GetChild(2).GetChild(0).GetComponent<SpriteRenderer>().color = colors[1]; // helmet
        sprites.GetChild(2).GetChild(1).GetComponent<SpriteRenderer>().color = colors[0]; // visor
    }





}
