using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class VirtualPlayer : NetworkBehaviour{

    public GameObject[] gamePlayerPrefabs;
    private GameObject _classSelection;
    public int classChoice;
    
    [SerializeField] private GameObject teammateStatusPrefab;
    [SerializeField] private GameObject scoreboard;
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

    public int health;
    private int _maxHealth;

    public int shield;
    private int _maxShield;
    
    [SyncVar] public int modePoints;
    [SyncVar] public int kills;
    [SyncVar] public int deaths;
    [SyncVar] public int score;

    private AudioManager _audioManager;
    
    
    private List<PlayerPanel> _scores = new List<PlayerPanel>();
    private GameObject _teamBoard;
    private GameObject _enemyTeamBoard;
    private int _enemyTeamSize;

    public override void OnStartClient(){
        _audioManager = GetComponent<AudioManager>();
        _classSelection = transform.GetChild(0).GetChild(2).gameObject;
        if (hasAuthority && SceneManager.GetActiveScene().buildIndex > 1){
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public override void OnStartServer(){
        base.OnStartServer();
        _networkManager = FindObjectOfType<CustomNetworkManager>();
    }

    
    
    
    [Client]
    private void InitializeTeammateStatuses(){
        if (teamIndex > 6){
            _teamBoard = scoreboard.transform.GetChild(1).gameObject;
            _enemyTeamBoard = scoreboard.transform.GetChild(0).gameObject;
        }
        else{
            _teamBoard = scoreboard.transform.GetChild(0).gameObject;
            _enemyTeamBoard = scoreboard.transform.GetChild(1).gameObject;
        }

        int position = -20;
        VirtualPlayer[] players = FindObjectsOfType<VirtualPlayer>();
        
        foreach (VirtualPlayer otherVirtualPlayer in players){
            if (otherVirtualPlayer.teamIndex < 7 && teamIndex < 7 || otherVirtualPlayer.teamIndex > 6 && teamIndex > 6){ // on same team
                if (this != otherVirtualPlayer){ // not this object
                    GameObject teammateStatus = CreateTeammateStatus(position);
                    Color helmetColor = otherVirtualPlayer.gamePlayer.helmet.GetChild(0).GetComponent<SpriteRenderer>().color;
                    Color visorColor = otherVirtualPlayer.gamePlayer.helmet.GetChild(1).GetComponent<SpriteRenderer>().color;

                    teammateStatus.transform.GetChild(0).GetComponent<Image>().color = helmetColor;
                    teammateStatus.transform.GetChild(1).GetComponent<Image>().color = visorColor; // visor
                    teammateStatus.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = otherVirtualPlayer.name; // name

                    Team.Add(new TeammateHUDElements(otherVirtualPlayer,
                        teammateStatus.transform.GetChild(4).GetComponent<TextMeshProUGUI>(), // health text
                        teammateStatus.transform.GetChild(3).GetComponent<Slider>(), // health slider
                        teammateStatus.transform.GetChild(6).GetComponent<TextMeshProUGUI>(), // shield text
                        teammateStatus.transform.GetChild(5).GetComponent<Slider>())); // shield slider
                    position -= 40;
                }

            }
            else{ // on different team
                _enemyTeamSize++;
            }
        }

        // dont need to fors.
        int index = 1;
        int enemyIndex = 1;
        foreach (VirtualPlayer otherVirtualPlayer in players){
            if (otherVirtualPlayer.teamIndex < 7 && teamIndex < 7 || otherVirtualPlayer.teamIndex > 6 && teamIndex > 6){ // on same team
                Transform playerPanel = _teamBoard.transform.GetChild(index);
                playerPanel.gameObject.SetActive(true);
                TextMeshProUGUI usrname = playerPanel.GetChild(0).GetComponent<TextMeshProUGUI>();
                usrname.text = otherVirtualPlayer.name;
                _scores.Add(new PlayerPanel(otherVirtualPlayer, usrname,
                    playerPanel.GetChild(1).GetComponent<TextMeshProUGUI>(),
                    playerPanel.GetChild(2).GetComponent<TextMeshProUGUI>(),
                    playerPanel.GetChild(3).GetComponent<TextMeshProUGUI>(),
                    playerPanel.GetChild(4).GetComponent<TextMeshProUGUI>()));
                index++;
            }
            
            else{ // on different team
                Transform playerPanel = _enemyTeamBoard.transform.GetChild(enemyIndex);
                playerPanel.gameObject.SetActive(true);
                TextMeshProUGUI usrname = playerPanel.GetChild(0).GetComponent<TextMeshProUGUI>();
                usrname.text = otherVirtualPlayer.name;
                _scores.Add(new PlayerPanel(otherVirtualPlayer, usrname, 
                    playerPanel.GetChild(1).GetComponent<TextMeshProUGUI>(), 
                    playerPanel.GetChild(2).GetComponent<TextMeshProUGUI>(), 
                    playerPanel.GetChild(3).GetComponent<TextMeshProUGUI>(), 
                    playerPanel.GetChild(4).GetComponent<TextMeshProUGUI>()));
                enemyIndex++;
            }
        }
        PlayerUpdateHUD();
    }
    private GameObject CreateTeammateStatus(int position){
        GameObject teammateStatus = Instantiate(teammateStatusPrefab, transform.GetChild(0).GetChild(0));
        teammateStatus.GetComponent<RectTransform>().anchoredPosition = new Vector3(65, position);
        
        return teammateStatus;  
    }
    
    
    
    
    
    
    
    
    
    public void SetLobbyPlayer(LobbyPlayer lobbyPlayer){
        _lobbyPlayer = lobbyPlayer;
    }

    public LobbyPlayer GetLobbyPlayer(){
        return _lobbyPlayer;
    }

    [Server]
    public void Respawn(){
        ClientRespawnTargetRpc(connectionToClient);
        StartCoroutine(ServerRespawn());
    }

    [TargetRpc]
    private void ClientRespawnTargetRpc(NetworkConnection connection){
        StartCoroutine(ClientRespawn());
    }

    [Client]
    private IEnumerator ClientRespawn(){
        yield return new WaitForSeconds(1);
        _classSelection.SetActive(true);
        yield return new WaitForSeconds(2);
        _audioManager.PlaySound(0);
        yield return new WaitForSeconds(1);
        
        _audioManager.PlaySound(0);
        yield return new WaitForSeconds(1);
        
        _audioManager.PlaySound(0);
        yield return new WaitForSeconds(1);
        
        _audioManager.PlaySound(1);
        yield return new WaitForSeconds(1);
        _classSelection.SetActive(false);

    }

    [Server]
    private IEnumerator ServerRespawn(){
        yield return new WaitForSeconds(7);
        if (gamePlayer){
            NetworkServer.Destroy(gamePlayer.gameObject);
        }

        _networkManager.SetupPlayer(connectionToClient, gamePlayerPrefabs[classChoice]);
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
        
        _maxHealth = player.maxHealth;
        _maxShield = player.maxShield;
        if (setUsername == ""){
            setUsername = "Player " + Random.Range(-9999999, 99999999);
        }

        gameObject.name = setUsername;
        player.gameObject.name = setUsername;
        Image bg = player.transform.GetChild(6).GetChild(0).GetComponent<Image>();
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
        player.HUDPickupWeapon(player.primaryWeapon);
        player.weaponImage.sprite = player.primaryWeapon.spriteRenderer.sprite;
        Transform sprites = gamePlayer.transform.GetChild(1);

        player.bodyRenderer.color = setColors[3]; // body
        player.armRenderer.color = setColors[2]; // arms armed
        player.helmetRenderer.color = setColors[1]; // helmet
        player.visorRenderer.color = setColors[0]; // visor

        player.abilityImage.sprite = player.abilityIcons[classChoice];
        Invoke(nameof(InitializeTeammateStatuses), 1f);

    }


    [ClientCallback]
    private void Update(){
        scoreboard.SetActive(false);
        if (Input.GetKey(KeyCode.Tab)){
            PlayerUpdateHUD();
            scoreboard.SetActive(true);
        }
        
          
        foreach (TeammateHUDElements teammateHUDElements in Team){
            int teammateHealth = teammateHUDElements.VirtualPlayer.health;
            int teammateMaxHealth = teammateHUDElements.VirtualPlayer._maxHealth;
            
            int teammateShield = teammateHUDElements.VirtualPlayer.shield;
            int teammateMaxShield = teammateHUDElements.VirtualPlayer._maxShield;
            
            if (teammateShield > 0){
                teammateHUDElements.HealthText.text = "";
                teammateHUDElements.ShieldText.text = $"{teammateShield}/{teammateMaxShield}";
            }
            else{
                teammateHUDElements.HealthText.text = $"{teammateHealth}/{teammateMaxHealth}";
                teammateHUDElements.ShieldText.text = "";
            }

            teammateHUDElements.HealthSlider.value = (float) teammateHealth / teammateMaxHealth;
            teammateHUDElements.ShieldSlider.value = (float) teammateShield / teammateMaxShield;

        }
    }

    [Client]
    private void PlayerUpdateHUD(){
        foreach (PlayerPanel panel in _scores){
            panel.ModePoints.text = "" + panel.VirtualPlayer.modePoints;
            panel.Kills.text = "" + panel.VirtualPlayer.kills;
            panel.Deaths.text = "" + panel.VirtualPlayer.deaths;
            panel.Score.text = "" + panel.VirtualPlayer.score;
        }
      

    }
    
        
    
    private class PlayerPanel{
        public VirtualPlayer VirtualPlayer;
        public TextMeshProUGUI Username;
        public TextMeshProUGUI ModePoints;
        public TextMeshProUGUI Kills;
        public TextMeshProUGUI Deaths;
        public TextMeshProUGUI Score;

        public PlayerPanel(VirtualPlayer virtualPlayer, TextMeshProUGUI username, TextMeshProUGUI modePoints, TextMeshProUGUI kills, TextMeshProUGUI deaths, TextMeshProUGUI score){
            VirtualPlayer = virtualPlayer;
            Username = username;
            ModePoints = modePoints;
            Kills = kills;
            Deaths = deaths;
            Score = score;
        }
    }
    
    [Client]
    public void SelectClass(int choice){
        classChoice = choice;
        CmdSetChoice(choice);
    }

    [Command]
    private void CmdSetChoice(int choice){
        classChoice = choice;
    }

}

public class TeammateHUDElements{
    public VirtualPlayer VirtualPlayer;
        
    public TextMeshProUGUI HealthText;
    public Slider HealthSlider;

    public TextMeshProUGUI ShieldText;
    public Slider ShieldSlider;
        
    public TeammateHUDElements(VirtualPlayer virtualPlayer, TextMeshProUGUI healthText, Slider healthSlider, TextMeshProUGUI shieldText, Slider shieldSlider){
        VirtualPlayer = virtualPlayer;
            
        HealthText = healthText;
        HealthSlider = healthSlider;

        ShieldText = shieldText;
        ShieldSlider = shieldSlider;
    }



// 0.3497704918 * unity units = 1 meter. 
}
