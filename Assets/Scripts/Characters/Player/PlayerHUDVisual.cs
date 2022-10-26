using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class Player : Character{

    [Header("Visuals")] [SerializeField] private GameObject spritesParent;
    [SerializeField] private Sprite[] ArmTypes;

    [SerializeField] private TextMeshProUGUI _notificationText;

    [Header("HUD")] [SerializeField] protected Canvas HUD;
    [SerializeField] private GameObject teammateStatusPrefab;
    [SerializeField] private GameObject scoreboard;
    [SerializeField] private TMP_Text pingDisplay;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider shieldSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI shieldText;

    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private GameObject shieldDamageSparks;
    [SerializeField] private GameObject armorDamageSparks;

    private Camera _mainCamera;
    private CinemachineVirtualCamera[] _virtualCameras;

    private GameObject _teamBoard;
    private GameObject _enemyTeamBoard;
    private int _enemyTeamSize;

    private List<PlayerPanel> _scores = new List<PlayerPanel>();

    // sprites
    public Transform arm;
    public Transform helmet;

    private SpriteRenderer _armRenderer;
    private bool _armOverrideReloading;

    private Color[] _colors = new Color[3];
    private GameObject[] sprites = new GameObject[7];


    //HUD
    private float _fadeTimer;
    private float fadeDelay = 50;
    private float fadeSpeed = 0.01f;

    public TextMeshProUGUI pickupText;

    public Image weaponImage;
    public TextMeshProUGUI ammoCounter;
    public TextMeshProUGUI magCounter;

    public TextMeshProUGUI energyCounter;
    public TextMeshProUGUI heatCounter;




    [Client]
    private void ClientHUDVisualStart(){

        if (hasAuthority){
            _mainCamera = Camera.main;
            _virtualCameras = new CinemachineVirtualCamera[3];
            _virtualCameras[0] = transform.GetChild(4).GetComponent<CinemachineVirtualCamera>();
            _virtualCameras[1] = transform.GetChild(5).GetComponent<CinemachineVirtualCamera>();
            _virtualCameras[2] = transform.GetChild(6).GetComponent<CinemachineVirtualCamera>();

            _virtualCameras[0].Priority = 10;
            HUD.gameObject.SetActive(true);


            // HUD
            weaponImage = HUD.transform.GetChild(2).GetComponent<Image>();
            ammoCounter = HUD.transform.GetChild(2).transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            magCounter = HUD.transform.GetChild(2).transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();

            energyCounter = HUD.transform.GetChild(2).transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
            heatCounter = HUD.transform.GetChild(2).transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();


            _notificationText.SetText("");
            pickupText = HUD.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            pickupText.SetText("");
        }

        _armRenderer = arm.GetChild(0).GetComponent<SpriteRenderer>();
        for (int i = 0; i < spritesParent.transform.childCount; i++){
            sprites[i] = spritesParent.transform.GetChild(i).gameObject;
        }

        
        if (teamIndex > 6){
            _teamBoard = scoreboard.transform.GetChild(1).gameObject;
            _enemyTeamBoard = scoreboard.transform.GetChild(0).gameObject;
        }
        else{
            _teamBoard = scoreboard.transform.GetChild(0).gameObject;
            _enemyTeamBoard = scoreboard.transform.GetChild(1).gameObject;
        }
    }

    /*
    [Client]
    private void InitializeTeammateStatuses(){
        int position = -20;
        VirtualPlayer[] players = FindObjectsOfType<VirtualPlayer>();
        
        foreach (VirtualPlayer otherVirtualPlayer in players){
            if (otherVirtualPlayer.teamIndex < 7 && teamIndex < 7 || otherVirtualPlayer.teamIndex > 6 && teamIndex > 6){ // on same team
                if (virtualPlayer != otherVirtualPlayer){
                    GameObject teammateStatus = CreateTeammateStatus(position);
                    Color helmetColor = otherVirtualPlayer.helmet.GetChild(0).GetComponent<SpriteRenderer>().color;
                    Color visorColor = otherVirtualPlayer.helmet.GetChild(1).GetComponent<SpriteRenderer>().color;

                    teammateStatus.transform.GetChild(0).GetComponent<Image>().color = helmetColor;
                    teammateStatus.transform.GetChild(1).GetComponent<Image>().color = visorColor; // visor
                    teammateStatus.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = otherVirtualPlayer.name; // name

                    virtualPlayer.Team.Add(new TeammateHUDElements(otherVirtualPlayer,
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
                TextMeshProUGUI username = playerPanel.GetChild(0).GetComponent<TextMeshProUGUI>();
                username.text = otherVirtualPlayer.name;
                _scores.Add(new PlayerPanel(otherVirtualPlayer, username,
                    playerPanel.GetChild(1).GetComponent<TextMeshProUGUI>(),
                    playerPanel.GetChild(2).GetComponent<TextMeshProUGUI>(),
                    playerPanel.GetChild(3).GetComponent<TextMeshProUGUI>(),
                    playerPanel.GetChild(4).GetComponent<TextMeshProUGUI>()));
                index++;
            }
            
            else{ // on different team
                Transform playerPanel = _enemyTeamBoard.transform.GetChild(enemyIndex);
                playerPanel.gameObject.SetActive(true);
                TextMeshProUGUI username = playerPanel.GetChild(0).GetComponent<TextMeshProUGUI>();
                username.text = otherVirtualPlayer.name;
                _scores.Add(new PlayerPanel(otherVirtualPlayer, username, 
                    playerPanel.GetChild(1).GetComponent<TextMeshProUGUI>(), 
                    playerPanel.GetChild(2).GetComponent<TextMeshProUGUI>(), 
                    playerPanel.GetChild(3).GetComponent<TextMeshProUGUI>(), 
                    playerPanel.GetChild(4).GetComponent<TextMeshProUGUI>()));
                enemyIndex++;
            }
        }
        PlayerUpdateHUD();
    }*/
    /*private GameObject CreateTeammateStatus(int position){
        GameObject teammateStatus = Instantiate(teammateStatusPrefab, HUD.transform.GetChild(5));
        teammateStatus.GetComponent<RectTransform>().anchoredPosition = new Vector3(65, position);
        
        return teammateStatus;  
    }*/
    private void ClientHUDUpdate(){ // every frame
        /*pingDisplay.text = Math.Round(NetworkTime.rtt * 1000) + " ms";
        foreach (TeammateHUDElements teammateHUDElements in virtualPlayer.Team){
            int teammateHealth = teammateHUDElements.VirtualPlayer.health;
            int teammateMaxHealth = teammateHUDElements.VirtualPlayer.MaxHealth;
            
            int teammateShield = teammateHUDElements.VirtualPlayer.shield;
            int teammateMaxShield = teammateHUDElements.VirtualPlayer.MaxShield;
            
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

        }*/
        
        scoreboard.SetActive(false);
        if (Input.GetKey(KeyCode.Tab)){
            PlayerUpdateHUD();
            scoreboard.SetActive(true);
        }
    }


    [Client]
    public override void HUDPickupWeapon(BasicWeapon weapon){ // called when smth changes like weapon swap
        _cursorControl.SetCursorType(weapon.cursorType);
        SetNotifText(weapon.name);
    }



    [ClientRpc]
    private void SetArmTypeClientRpc(int armyType){
        SetArmType(armyType);
    }
    
    private void SetArmType(int armType){
        _armRenderer.sprite = ArmTypes[armType];
    }
    
    public override void Reload(){ // for reloading and holding melee
        _armOverrideReloading = true;
    }

    public override void FinishReload(){
        _armOverrideReloading = false;
    }

    
    
    private void SetColor(float r, float g, float b, float a){
        Color color = _notificationText.color;
        _notificationText.color = new Color(color.r + r, color.g + g, color.b + b, a);
    }
    public void SetNotifText(string setText){
        _notificationText.SetText(setText);
        SetColor(0, 0, 0, 1);
        _fadeTimer = fadeDelay;
    }
    public void SetPickupText(string setText){
        pickupText.SetText(setText);
    }

    [Client]
    private void PlayerUpdateHUD(){
        foreach (PlayerPanel score in _scores){
            score.ModePoints.text = "" + score.VirtualPlayer.modePoints;
            score.Kills.text = "" + score.VirtualPlayer.kills;
            score.Deaths.text = "" + score.VirtualPlayer.deaths;
            score.Score.text = "" + score.VirtualPlayer.score;
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

}