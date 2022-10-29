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

    private Dictionary<NetworkConnectionToClient, Color[]> _colors = new Dictionary<NetworkConnectionToClient, Color[]>();
    public Dictionary<NetworkConnectionToClient, string> Usernames = new Dictionary<NetworkConnectionToClient, string>();
    public Dictionary<NetworkConnectionToClient, int> TeamIndices = new Dictionary<NetworkConnectionToClient, int>();

    public bool allPlayersReady;

    private List<Vector3> _blueSpawns = new List<Vector3>();
    private int _blueSpawnCounter;
    private List<Vector3> _redSpawns = new List<Vector3>();
    private int _redSpawnCounter;
    
    
    public override void OnServerAddPlayer(NetworkConnectionToClient conn){
        base.OnServerAddPlayer(conn);
        string sceneName = SceneManager.GetActiveScene().name;
        
        // if in the lobby, spawn a lobby character
        if (sceneName == "Lobby"){
            LobbyPlayer lobbyPlayer = Instantiate(lobbyPlayerPrefab, new Vector3(10000, 10000, 0), Quaternion.identity, FindObjectOfType<Canvas>().transform).GetComponent<LobbyPlayer>();

            conn.identity.GetComponent<VirtualPlayer>().SetLobbyPlayer(lobbyPlayer);

            NetworkServer.Spawn(lobbyPlayer.gameObject, conn);

            _colors.Add(conn, null);
            Usernames.Add(conn, null);
            TeamIndices.Add(conn, 0);
        }
    }

    public override void OnServerSceneChanged(string sceneName){
        if (sceneName != "Assets/Scenes/Lobby.unity" && sceneName != "Assets/Scenes/Menu.unity"){
            TeamsSpawns[] teamsSpawnsArray = FindObjectsOfType<TeamsSpawns>();
            _blueSpawns = teamsSpawnsArray[0].GetSpawns();
            _redSpawns = teamsSpawnsArray[1].GetSpawns();
            StartCoroutine(SetupPlayers());
        }
    }

    private IEnumerator SetupPlayers(){
        
        foreach (NetworkConnectionToClient networkConnectionToClient in NetworkServer.connections.Values){
            yield return new WaitUntil(() => networkConnectionToClient.isReady);
        }

        
        foreach (NetworkConnectionToClient connectionToClient in NetworkServer.connections.Values){
            connectionToClient.identity.GetComponent<VirtualPlayer>().Respawn();
            //SetupPlayer(connectionToClient, connectionToClient.identity.GetComponent<VirtualPlayer>().gamePlayerPrefab);
        }

        yield return new WaitForSeconds(Time.fixedDeltaTime);
        FindObjectOfType<ModeManager>().AllPlayersReady();
        allPlayersReady = true;
    }


    [Server]
    public void SetupPlayer(NetworkConnectionToClient connectionToClient, GameObject prefab){
        
        Player player = Instantiate(prefab).GetComponent<Player>();
        BasicWeapon pW = Instantiate(player.primaryWeaponPrefab);
        BasicWeapon sW = Instantiate(player.secondaryWeaponPrefab);

        // cuts "(clone)" off the end
        pW.name = pW.name.Remove(pW.name.Length - 7);
        sW.name = sW.name.Remove(sW.name.Length - 7);

        NetworkServer.Spawn(player.gameObject, connectionToClient);
        NetworkServer.Spawn(pW.gameObject, connectionToClient);
        NetworkServer.Spawn(sW.gameObject, connectionToClient);


        player.primaryWeapon = pW;
        player.secondaryWeapon = sW;
        player.InitializeWeaponsOnClient(pW, sW);
        
        pW.StartCoroutine(pW.ServerInitializeWeapon(true, player, new []{1, 3}));
        sW.StartCoroutine(sW.ServerInitializeWeapon(false, player, new []{1, 3}));

        int teamIndex = TeamIndices[connectionToClient];
        if (teamIndex > 6){
            player.transform.position = _redSpawns[_redSpawnCounter];
            _redSpawnCounter++;
            if (_redSpawnCounter >= _redSpawns.Count){
                _redSpawnCounter = 0;
            }
        }
        
        else{
            player.transform.position = _blueSpawns[_blueSpawnCounter];
            _blueSpawnCounter++;
            if (_blueSpawnCounter >= _blueSpawns.Count){
                _blueSpawnCounter = 0;
            }
        }
        connectionToClient.identity.GetComponent<VirtualPlayer>().SetupPlayer(player, Usernames[connectionToClient], _colors[connectionToClient], teamIndex);

    }


    public void NetworkManagerSetColors(NetworkConnectionToClient connectionToClient, Color[] colors){
        _colors[connectionToClient] = colors;
    }
    

    public void NetworkManagerSetUsername(NetworkConnectionToClient connectionToClient, string username){
        Usernames[connectionToClient] = username;
    }
    

    public void NetworkManagerSetTeamIndex(NetworkConnectionToClient connectionToClient, int teamIndex){
        TeamIndices[connectionToClient] = teamIndex;
    }
    
    

}
