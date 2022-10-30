using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class Player : Character{

    [Header("Visuals")]
    [SerializeField] private Sprite[] armTypes;
    [SerializeField] private TextMeshProUGUI notificationText;

    [Header("HUD")] [SerializeField] protected Canvas hud;
    [SerializeField] private TMP_Text pingDisplay;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider shieldSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI shieldText;

    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private GameObject shieldDamageSparks;
    [SerializeField] private GameObject armorDamageSparks;

    public Sprite[] abilityIcons;
    [SerializeField] private Slider abilityChargeSlider;
    public Image abilityImage;

    
    
    [Header("Weapon HUD")]
    public TextMeshProUGUI pickupText;

    public Image weaponImage;
    public TextMeshProUGUI ammoCounter;
    public TextMeshProUGUI magCounter;

    public TextMeshProUGUI energyCounter;
    public TextMeshProUGUI heatCounter;
    

    [Header("Sprites")]
    public Transform arm;
    public Transform helmet;

    public SpriteRenderer bodyRenderer;
    public SpriteRenderer armRenderer;
    public SpriteRenderer helmetRenderer;
    public SpriteRenderer visorRenderer;

    private bool _armOverrideReloading;
    private bool _armOverrideSprinting;
    //HUD
    private float _fadeTimer;
    private float fadeDelay = 50;
    private float fadeSpeed = 0.01f;

    private Camera _mainCamera;
    private CinemachineVirtualCamera[] _virtualCameras;


    [Client]
    private void ClientHUDVisualStart(){

        if (hasAuthority){
            _mainCamera = Camera.main;
            _virtualCameras = new CinemachineVirtualCamera[3];
            _virtualCameras[0] = transform.GetChild(4).GetComponent<CinemachineVirtualCamera>();
            _virtualCameras[1] = transform.GetChild(5).GetComponent<CinemachineVirtualCamera>();
            _virtualCameras[2] = transform.GetChild(6).GetComponent<CinemachineVirtualCamera>();

            _virtualCameras[0].Priority = 10;
            hud.gameObject.SetActive(true);


            // HUD
            weaponImage = hud.transform.GetChild(2).GetComponent<Image>();
            ammoCounter = hud.transform.GetChild(2).transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            magCounter = hud.transform.GetChild(2).transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();

            energyCounter = hud.transform.GetChild(2).transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
            heatCounter = hud.transform.GetChild(2).transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();


            notificationText.SetText("");
            pickupText = hud.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            pickupText.SetText("");
        }

        Transform p = transform.GetChild(1);

    }

    
    private void ClientHUDUpdate(){ // every frame
        pingDisplay.text = Math.Round(NetworkTime.rtt * 1000) + " ms";
        if (_fadeTimer > 0){
            _fadeTimer --;   
        }
        else{
            float alpha = notificationText.color.a;
            if (alpha > 0){
                SetColor(0, 0, 0, notificationText.color.a - fadeSpeed);
            }
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
        armRenderer.sprite = armTypes[armType];
    }
    
    public override void Reload(){ // for reloading and holding melee
        _armOverrideReloading = true;
    }

    public override void FinishReload(){
        _armOverrideReloading = false;
    }

    
    
    private void SetColor(float r, float g, float b, float a){
        Color color = notificationText.color;
        notificationText.color = new Color(color.r + r, color.g + g, color.b + b, a);
    }
    public void SetNotifText(string setText){
        notificationText.SetText(setText);
        SetColor(0, 0, 0, 1);
        _fadeTimer = fadeDelay;
    }
    public void SetPickupText(string setText){
        pickupText.SetText(setText);
    }

    
    private class ANames{
        public readonly string running = "Running";
        public readonly  string runningBackwards = "RunningBackward";
        public readonly  string crouching = "Crouching";
        public readonly  string jumping = "Jumping";
        public readonly  string punching = "Punch";
        public readonly  string dying = "Dying";
    }

    

}