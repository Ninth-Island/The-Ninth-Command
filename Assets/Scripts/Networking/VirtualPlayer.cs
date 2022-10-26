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
    [SerializeField] private Material redTeamMaterial;
    [SerializeField] private Material blueTeamMaterial;
    [SerializeField] private Material noTeamMaterial;
    private LobbyPlayer _lobbyPlayer;
    private Player _gamePlayer;

    private Material _outline;
    
    
    [SyncVar] public int modePoints;
    [SyncVar] public int kills;
    [SyncVar] public int deaths;
    [SyncVar] public int score;

    public void SetLobbyPlayer(LobbyPlayer lobbyPlayer){
        _lobbyPlayer = lobbyPlayer;
    }

    public LobbyPlayer GetLobbyPlayer(){
        return _lobbyPlayer;
    }


    [Server]
    public void SetupPlayer(Player player, string username, Color[] colors, int teamIndex){
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
        player.virtualPlayer = this;
        if (username == ""){
            username = "Player " + Random.Range(-9999999, 99999999);
        }

        gameObject.name = username;
        player.gameObject.name = username;
        Image bg = player.transform.GetChild(8).GetChild(0).GetComponent<Image>();
        TextMeshProUGUI floatingName = bg.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        floatingName.text = username;


        _outline = noTeamMaterial;
        if (teamIndex > 6){
            player.SetLayer(LayerMask.NameToLayer("Team 2"));
            _outline = redTeamMaterial;
            if (hasAuthority){
                floatingName.color = Color.magenta;
            }
            else{
                floatingName.color = Color.red;
            }
        }
        else if (teamIndex > 0){
            player.SetLayer(LayerMask.NameToLayer("Team 1"));
            _outline = blueTeamMaterial;
            if (hasAuthority){
                floatingName.color = Color.green;
            }
            else{
                floatingName.color = Color.cyan;
            }
        }

        player.teamIndex = teamIndex;

        Transform sprites = _gamePlayer.transform.GetChild(1);

        SetupSpriteRenderer(sprites.GetChild(0).GetComponent<SpriteRenderer>(), colors[3]); // body
        SetupSpriteRenderer(sprites.GetChild(1).GetComponent<SpriteRenderer>(), colors[2]); // arms unarmed
        SetupSpriteRenderer(sprites.GetChild(3).GetChild(0).GetComponent<SpriteRenderer>(), colors[2]); // arms armed
        SetupSpriteRenderer(sprites.GetChild(2).GetChild(0).GetComponent<SpriteRenderer>(), colors[1]); // helmet
        SetupSpriteRenderer(sprites.GetChild(2).GetChild(1).GetComponent<SpriteRenderer>(), colors[0]); // visor
    }

    private void SetupSpriteRenderer(SpriteRenderer spriteRenderer, Color color){
        spriteRenderer.color = color;
        //spriteRenderer.material = _outline;
    }




// 0.3497704918 * unity units = 1 meter. 
}
