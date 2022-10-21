using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class Player : Character
{
    
    [Header("Visuals")]
    [SerializeField] private GameObject spritesParent;
    [SerializeField] private Sprite[] ArmTypes;

    [SerializeField] private TextMeshProUGUI _notificationText;

    [Header("HUD")]
    [SerializeField] protected Canvas HUD;
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

    private void ClientHUDUpdate(){ // every frame
        pingDisplay.text = Math.Round(NetworkTime.rtt * 1000) + " ms";
    }


    [Client]
    public override void HUDPickupWeapon(){ // called when smth changes like weapon swap
        _cursorControl.SetCursorType(primaryWeapon.cursorType);

        SetNotifText(primaryWeapon.name);
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
}
