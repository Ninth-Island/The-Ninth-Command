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


    [Header("HUD")]
    [SerializeField] protected Canvas HUD;
    [SerializeField] private TMP_Text pingDisplay;

    
    
    private Camera _mainCamera;
    private CinemachineVirtualCamera[] _virtualCameras;

    
    // sprites
    public Transform arm;
    public Transform helmet;

    private SpriteRenderer _armRenderer;
    private bool _armOverride;
    
    private Color[] _colors = new Color[3];
    private GameObject[] sprites = new GameObject[7];

    
    
    //HUD
    private TextMeshProUGUI _notificationText;
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

    
            _notificationText = HUD.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            _notificationText.SetText("");
            pickupText = HUD.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            pickupText.SetText("");
    
    
            Transform energyGauge = HUD.transform.GetChild(4);
            _energySlider = energyGauge.GetChild(2).GetComponent<Slider>();
            _overflowSlider = energyGauge.GetChild(3).GetComponent<Slider>();

        }

        
        _armRenderer = arm.GetChild(0).GetComponent<SpriteRenderer>();
        for (int i = 0; i < spritesParent.transform.childCount; i++){
            sprites[i] = spritesParent.transform.GetChild(i).gameObject;
        }
        
    }

    private void ClientHUDUpdate(){ // every frame
        pingDisplay.text = Math.Round(NetworkTime.rtt * 1000) + " ms";
    }

    [ClientRpc]
    public override void HUDPickupWeapon(){ // called when smth changes like weapon swap
<<<<<<< HEAD
        if (hasAuthority){
            weaponImage.sprite = primaryWeapon.spriteRenderer.sprite;
            _cursorControl.SetCursorType(primaryWeapon.cursorType);
=======
        weaponImage.sprite = primaryWeapon.spriteRenderer.sprite;
        _cursorControl.SetCursorType(primaryWeapon.cursorType);
>>>>>>> 2d66961aea6a644ab4ea4b5a77be714cf394d70f

            SetNotifText(primaryWeapon.name);
        }

        SetArmType(primaryWeapon.armType);
    }

    

    public void SetArmType(int armType){
        _armRenderer.sprite = ArmTypes[armType];
    }

    
    public override void Reload(){ // for reloading and holding melee
        _armOverride = true;
    }

    public override void FinishReload(){
        _armOverride = false;
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
<<<<<<< HEAD
=======
    
>>>>>>> 2d66961aea6a644ab4ea4b5a77be714cf394d70f
}
