using System.Collections;
using System.Collections.Generic;
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
    
    
    
    private Transform _arm;
    public Transform Helmet;

    private SpriteRenderer _armRenderer;
    private bool _armOverride;
    
    private Color[] _colors = new Color[3];
    private GameObject[] sprites = new GameObject[7];



    private void HUDVisualStart(){
        
        
        // for the sprites that rotate and recolor and stuff

        _arm = transform.GetChild(1).transform.GetChild(5);
        Helmet = transform.GetChild(1).transform.GetChild(4);

        _armRenderer = _arm.GetChild(0).GetComponent<SpriteRenderer>();
        spriteLayer = _armRenderer.sortingLayerID;

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
        
        
        Transform energyGauge = HUD.transform.GetChild(5);
        _energySlider = energyGauge.GetChild(2).GetComponent<Slider>();
        _overflowSlider = energyGauge.GetChild(3).GetComponent<Slider>();

    }

    private void UpdateHUD(){
        weaponImage.sprite = primaryWeapon.spriteRenderer.sprite;
        _cursorControl.SetCursorType(primaryWeapon.cursorType);

        primaryWeapon.AudioManager.PlaySound(2, false);


        primaryWeapon.transform.localPosition = primaryWeapon.offset;

        SetNotifText(primaryWeapon.name);
        SetArmType(primaryWeapon.armType);
    }

    
    
    private void RotateArm(){
        float rotation = GetBarrelToMouseRotation();

        if (_armOverride == false){
            _arm.transform.rotation = Quaternion.Euler(0, 0, rotation);
            _arm.transform.localScale = new Vector3(1, 1);
        }
        else{
            _arm.transform.localRotation = Quaternion.Euler(0, 0, -30);
            if (Mathf.Sign(transform.localScale.x) < 0){
                _arm.transform.rotation = Quaternion.Euler(0, 0, -150);
    
            }
            _arm.transform.localScale = new Vector3(1, 1);
        }

        Helmet.transform.rotation = Quaternion.Euler(0, 0, rotation);
        Helmet.transform.localScale = new Vector3(1, 1);

        
        transform.localScale = new Vector3(1, 1);
        if (rotation > 90 && rotation < 270){
            _arm.transform.localScale = new Vector3(-1, -1);
            Helmet.transform.localScale = new Vector3(-1, -1);
            transform.localScale = new Vector3(-1, 1);
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
