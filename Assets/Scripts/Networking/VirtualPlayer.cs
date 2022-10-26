using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class VirtualPlayer : NetworkBehaviour{

    public GameObject gamePlayerPrefab;
    [SerializeField] private Material redTeamMaterial;
    [SerializeField] private Material blueTeamMaterial;
    [SerializeField] private Material noTeamMaterial;
    private LobbyPlayer _lobbyPlayer;
    public Player gamePlayer;
    private CustomNetworkManager _networkManager;
    public List<TeammateHUDElements> Team = new List<TeammateHUDElements>();

    private Material _outline;

    public string username;
    public Color[] colors;
    public int teamIndex;
    
    [SyncVar] public int modePoints;
    [SyncVar] public int kills;
    [SyncVar] public int deaths;
    [SyncVar] public int score;

    private AudioManager _audioManager;

    public override void OnStartClient(){
        _audioManager = GetComponent<AudioManager>();
    }

    public override void OnStartServer(){
        base.OnStartServer();
        _networkManager = FindObjectOfType<CustomNetworkManager>();
    }

    public void SetLobbyPlayer(LobbyPlayer lobbyPlayer){
        _lobbyPlayer = lobbyPlayer;
    }

    public LobbyPlayer GetLobbyPlayer(){
        return _lobbyPlayer;
    }

    [Server]
    public void Respawn(Vector3 position){
        ClientRespawnTargetRpc(connectionToClient);
        StartCoroutine(ServerRespawn());
    }

    [TargetRpc]
    private void ClientRespawnTargetRpc(NetworkConnection connection){
        StartCoroutine(ClientRespawn());
    }

    [Client]
    private IEnumerator ClientRespawn(){
        
        yield return new WaitForSeconds(3);
        _audioManager.PlaySound(0);
        yield return new WaitForSeconds(1);
        
        _audioManager.PlaySound(0);
        yield return new WaitForSeconds(1);
        
        _audioManager.PlaySound(0);
        yield return new WaitForSeconds(1);
        
        _audioManager.PlaySound(1);
        yield return new WaitForSeconds(1);

    }

    [Server]
    private IEnumerator ServerRespawn(){
        yield return new WaitForSeconds(7);
        NetworkServer.Destroy(gamePlayer.gameObject);
        _networkManager.SetupPlayer(connectionToClient, gamePlayerPrefab);
    }


    [Server]
    public void SetupPlayer(Player player, string setUsername, Color[] setColors, int setTeamIndex){
        username = setUsername;
        colors = setColors;
        teamIndex = setTeamIndex;
        ClientSetupPlayer(player, setUsername, setColors, setTeamIndex);
        if (setTeamIndex > 6){
            player.SetLayer(LayerMask.NameToLayer("Team 2"));
        }
        else{
            player.SetLayer(LayerMask.NameToLayer("Team 1"));
        }

    }

    [ClientRpc]
    private void ClientSetupPlayer(Player player, string setUsername, Color[] setColors, int setTeamIndex){
        gamePlayer = player;
        username = setUsername;
        colors = setColors;
        teamIndex = setTeamIndex;
        
        player.virtualPlayer = this;
        if (setUsername == ""){
            setUsername = "Player " + Random.Range(-9999999, 99999999);
        }

        gameObject.name = setUsername;
        player.gameObject.name = setUsername;
        Image bg = player.transform.GetChild(8).GetChild(0).GetComponent<Image>();
        TextMeshProUGUI floatingName = bg.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        floatingName.text = setUsername;


        _outline = noTeamMaterial;
        if (setTeamIndex > 6){
            player.SetLayer(LayerMask.NameToLayer("Team 2"));
            _outline = redTeamMaterial;
            if (hasAuthority){
                floatingName.color = Color.magenta;
            }
            else{
                floatingName.color = Color.red;
            }
        }
        else if (setTeamIndex > 0){
            player.SetLayer(LayerMask.NameToLayer("Team 1"));
            _outline = blueTeamMaterial;
            if (hasAuthority){
                floatingName.color = Color.green;
            }
            else{
                floatingName.color = Color.cyan;
            }
        }

        player.teamIndex = setTeamIndex;

        Transform sprites = gamePlayer.transform.GetChild(1);

        SetupSpriteRenderer(sprites.GetChild(0).GetComponent<SpriteRenderer>(), setColors[3]); // body
        SetupSpriteRenderer(sprites.GetChild(1).GetComponent<SpriteRenderer>(), setColors[2]); // arms unarmed
        SetupSpriteRenderer(sprites.GetChild(3).GetChild(0).GetComponent<SpriteRenderer>(), setColors[2]); // arms armed
        SetupSpriteRenderer(sprites.GetChild(2).GetChild(0).GetComponent<SpriteRenderer>(), setColors[1]); // helmet
        SetupSpriteRenderer(sprites.GetChild(2).GetChild(1).GetComponent<SpriteRenderer>(), setColors[0]); // visor
    }

    private void SetupSpriteRenderer(SpriteRenderer spriteRenderer, Color color){
        spriteRenderer.color = color;
        //spriteRenderer.material = _outline;
    }




// 0.3497704918 * unity units = 1 meter. 
}
