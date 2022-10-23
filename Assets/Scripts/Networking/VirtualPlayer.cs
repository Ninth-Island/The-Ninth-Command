using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    public void SetupPlayer(Player player, string username, Color[] colors, List<Vector3> spawns, int teamIndex){
        ClientSetupPlayer(player, username, colors, teamIndex);
        if (teamIndex > 6){
            player.SetLayer(LayerMask.NameToLayer("Team 2"));
        }
        else{
            player.SetLayer(LayerMask.NameToLayer("Team 1"));
        }

    }

    [ClientRpc]
    private void ClientSetupPlayer(Player player, string username, Color[] colors, int teamIndex){
        _gamePlayer = player;
        if (username == ""){
            username = "Player " + Random.Range(-9999999, 99999999);
        }
        gameObject.name = username;
        player.gameObject.name = username;
        Image bg = player.transform.GetChild(8).GetChild(0).GetComponent<Image>();
        TextMeshProUGUI floatingName = bg.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        floatingName.text = username;

        if (teamIndex > 6){
            player.SetLayer(LayerMask.NameToLayer("Team 2"));
            if (hasAuthority){
                floatingName.color = Color.magenta;
            }
            else{
                floatingName.color = Color.red;
            }
        } 
        else if (teamIndex > 0){
            player.SetLayer(LayerMask.NameToLayer("Team 1"));
            if (hasAuthority){
                floatingName.color = Color.green; 
            }
            else{
                floatingName.color = Color.cyan;
            }
        }

        player.teamIndex = teamIndex;
        
        Transform sprites = _gamePlayer.transform.GetChild(1);
        sprites.GetChild(0).GetComponent<SpriteRenderer>().color = colors[3]; // body
        
        sprites.GetChild(1).GetComponent<SpriteRenderer>().color = colors[2]; // arms unarmed
        sprites.GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().color = colors[2]; // arms armed
        
        sprites.GetChild(2).GetChild(0).GetComponent<SpriteRenderer>().color = colors[1]; // helmet
        sprites.GetChild(2).GetChild(1).GetComponent<SpriteRenderer>().color = colors[0]; // visor
    }





}
