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
    private Dictionary<NetworkConnectionToClient, string> _usernames = new Dictionary<NetworkConnectionToClient, string>();
    private Dictionary<NetworkConnectionToClient, int> _teamIndices = new Dictionary<NetworkConnectionToClient, int>();

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
            _usernames.Add(conn, null);
            _teamIndices.Add(conn, 0);
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
            SetupPlayer(connectionToClient);
        }

        yield return new WaitForSeconds(Time.fixedDeltaTime);
        allPlayersReady = true;
    }


    [Server]
    private void SetupPlayer(NetworkConnectionToClient connectionToClient){
        
        Player player = Instantiate(gamePlayerPrefab).GetComponent<Player>();
        BasicWeapon pW = Instantiate(player.primaryWeaponPrefab);
        BasicWeapon sW = Instantiate(player.secondaryWeaponPrefab);

        // cuts "(clone)" off the end
        pW.name = pW.name.Remove(pW.name.Length - 7);
        sW.name = sW.name.Remove(sW.name.Length - 7);
        
        // this is server only for hierarchy organization
        Transform container = new GameObject($"Player {connectionToClient.connectionId}").transform;
        player.transform.parent = container;
        pW.transform.parent = container;
        sW.transform.parent = container;

        NetworkServer.Spawn(player.gameObject, connectionToClient);
        NetworkServer.Spawn(pW.gameObject, connectionToClient);
        NetworkServer.Spawn(sW.gameObject, connectionToClient);


        player.primaryWeapon = pW;
        player.secondaryWeapon = sW;
        player.InitializeWeaponsOnClient(pW, sW);
        
        pW.StartCoroutine(pW.ServerInitializeWeapon(true, player, new []{1, 3}));
        sW.StartCoroutine(sW.ServerInitializeWeapon(false, player, new []{1, 3}));

        int teamIndex = _teamIndices[connectionToClient];
        List<Vector3> spawns;
        if (teamIndex > 6){
            player.transform.position = _redSpawns[_redSpawnCounter];
            spawns = _redSpawns;
            _redSpawnCounter++;
        }
        else{
            player.transform.position = _blueSpawns[_blueSpawnCounter];
            spawns = _blueSpawns;
            _blueSpawnCounter++;
        }
        connectionToClient.identity.GetComponent<VirtualPlayer>().SetupPlayer(player, _usernames[connectionToClient], _colors[connectionToClient], spawns, teamIndex);

    }


    public void NetworkManagerSetColors(NetworkConnectionToClient connectionToClient, Color[] colors){
        _colors[connectionToClient] = colors;
    }
    

    public void NetworkManagerSetUsername(NetworkConnectionToClient connectionToClient, string username){
        _usernames[connectionToClient] = username;
    }
    

    public void NetworkManagerSetTeamIndex(NetworkConnectionToClient connectionToClient, int teamIndex){
        _teamIndices[connectionToClient] = teamIndex;
    }
    
    

}
