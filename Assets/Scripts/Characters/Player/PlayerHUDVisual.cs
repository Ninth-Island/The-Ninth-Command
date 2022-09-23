using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class Player : Character
{
    
    [Header("Visuals")]
    [SerializeField] private GameObject spritesParent;

    [SerializeField] private Sprite[] ArmTypes;


    [Header("HUD and Visuals")]
    [SerializeField] protected Canvas HUD;
    [SerializeField] private TMP_Text pingDisplay;
    
    

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
    
    
    
    public Transform arm;
    public Transform helmet;

    private SpriteRenderer _armRenderer;
    private bool _armOverride;
    
    private Color[] _colors = new Color[3];
    private GameObject[] sprites = new GameObject[7];

    

    [Client]
    private void HUDVisualStart(){
        _armRenderer = arm.GetChild(0).GetComponent<SpriteRenderer>();
        for (int i = 0; i < spritesParent.transform.childCount; i++){
            sprites[i] = spritesParent.transform.GetChild(i).gameObject;
        }
        
        
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

    private void HUDUpdate(){ // every frame
        pingDisplay.text = Math.Round(NetworkTime.rtt * 1000) + " ms";
    }

    [ClientRpc]
    public override void UpdateHUD(){ // called when smth changes like weapon swap
        weaponImage.sprite = primaryWeapon.spriteRenderer.sprite;
        _cursorControl.SetCursorType(primaryWeapon.cursorType);


        SetNotifText(primaryWeapon.name);
        SetArmType(primaryWeapon.armType);
    }

    
    
    [Command]
    private void CmdRotateArm(float rotation){
        if (_armOverride == false){
            arm.transform.rotation = Quaternion.Euler(0, 0, rotation);
            arm.transform.localScale = new Vector3(1, 1);
        }
        else{
            arm.transform.localRotation = Quaternion.Euler(0, 0, -30);
            if (Mathf.Sign(transform.localScale.x) < 0){
                arm.transform.rotation = Quaternion.Euler(0, 0, -150);
    
            }
            arm.transform.localScale = new Vector3(1, 1);
        }

        helmet.transform.rotation = Quaternion.Euler(0, 0, rotation);
        helmet.transform.localScale = new Vector3(1, 1);


        if (rotation > 90 && rotation < 270){
            arm.transform.localScale = new Vector3(-1, -1);
            helmet.transform.localScale = new Vector3(-1, -1);
            transform.localScale = new Vector3(-1, 1);
        }
        else{
            transform.localScale = new Vector3(1, 1);
        }
    }

    public void SetArmType(int armType){
        _armRenderer.sprite = ArmTypes[armType];
    }

    
    public override void Reload(){ // for melee weapons
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
    
    private void Transform(float x){ 
        Vector2 pos = transform.position;
        transform.position = new Vector3(pos.x + x * transform.localScale.x, pos.y);
    }
}
