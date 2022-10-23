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


    private List<TeammateHUDElements> _team = new List<TeammateHUDElements>();


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
    }


    private void InitializeTeammateStatuses(){
        int position = 130;
        if (teamIndex < 6){
            foreach (Player player in FindObjectsOfType<Player>()){
                if (player.teamIndex < 6 && player != this){
                    GameObject teammateStatus = CreateTeammateStatus(position);

                    teammateStatus.transform.GetChild(0).GetComponent<Image>().color = player.helmet.GetChild(0).GetComponent<SpriteRenderer>().color; // helmet
                    teammateStatus.transform.GetChild(1).GetComponent<Image>().color = player.helmet.GetChild(1).GetComponent<SpriteRenderer>().color; // visor
                    teammateStatus.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = player.name; // name
                    
                    _team.Add(new TeammateHUDElements(player, 
                        teammateStatus.transform.GetChild(4).GetComponent<TextMeshProUGUI>(), // health text
                        teammateStatus.transform.GetChild(3).GetComponent<Slider>(), // health slider
                        teammateStatus.transform.GetChild(6).GetComponent<TextMeshProUGUI>(), // shield text
                        teammateStatus.transform.GetChild(5).GetComponent<Slider>())); // shield slider
                    position += 255;
                }
            }
        }
        if (teamIndex >= 6){
            foreach (Player player in FindObjectsOfType<Player>()){
                if (player.teamIndex >= 6 && player != this){
                    
                }
            }
        }
    }
    private GameObject CreateTeammateStatus(int position){
        GameObject teammateStatus = Instantiate(teammateStatusPrefab, HUD.transform.GetChild(6));
        teammateStatus.GetComponent<RectTransform>().anchoredPosition = new Vector3(position, -40);
        
        return teammateStatus;  
    }
    private void ClientHUDUpdate(){ // every frame
        pingDisplay.text = Math.Round(NetworkTime.rtt * 1000) + " ms";
        foreach (TeammateHUDElements teammateHUDElements in _team){
            int teammateHealth = teammateHUDElements.Player.health;
            int teammateMaxHealth = teammateHUDElements.Player.MaxHealth;
            
            int teammateShield = teammateHUDElements.Player.shield;
            int teammateMaxShield = teammateHUDElements.Player.MaxShield;
            
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

    private class TeammateHUDElements{
        public Player Player;
        
        public TextMeshProUGUI HealthText;
        public Slider HealthSlider;

        public TextMeshProUGUI ShieldText;
        public Slider ShieldSlider;
        
        public TeammateHUDElements(Player player, TextMeshProUGUI healthText, Slider healthSlider, TextMeshProUGUI shieldText, Slider shieldSlider){
            Player = player;

            HealthText = healthText;
            HealthSlider = healthSlider;

            ShieldText = shieldText;
            ShieldSlider = shieldSlider;
        }

    }
}
