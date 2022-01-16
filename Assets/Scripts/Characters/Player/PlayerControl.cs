using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class Player : Character{
    
    /*
* ================================================================================================================
*                                               PlayerControl
*
*  Parentally unrelated to player or character.
     *
     * Contains HUD (canvas) control for updating HUD text and getting mouse position
*
* 
* ================================================================================================================
*/
    
    [SerializeField] protected Canvas HUD;
    

    private TextMeshProUGUI _notificationText;
    private float _fadeTimer;
    private float fadeDelay = 50;
    private float fadeSpeed = 0.01f;

    public TextMeshProUGUI pickupText;
    private Camera _mainCamera;

    public Image WeaponImage;
    public TextMeshProUGUI ammoCounter;
    public TextMeshProUGUI magCounter;
    
    public TextMeshProUGUI energyCounter;
    public TextMeshProUGUI heatCounter;

    private CursorControl _cursorControl;



    // Start is called before the first frame update
    protected virtual void Start2(){

        WeaponImage = HUD.transform.GetChild(2).GetComponent<Image>();
        ammoCounter = HUD.transform.GetChild(2).transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        magCounter = HUD.transform.GetChild(2).transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        
        energyCounter = HUD.transform.GetChild(2).transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        heatCounter = HUD.transform.GetChild(2).transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();

        
        _notificationText = HUD.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _notificationText.SetText("");
        pickupText = HUD.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        pickupText.SetText("");
        
        _mainCamera = Camera.main;
        _cursorControl = FindObjectOfType<CursorControl>();
    }

    /*
* ================================================================================================================
*                                               Functions
*
*  Just separating because there's a lot of variables up there
*
* 
* ================================================================================================================
*/
    // Update is called once per frame
    protected virtual void Update2(){
        if (_fadeTimer > 0){
            _fadeTimer --;   
        }
        else{
            float alpha = _notificationText.color.a;
            if (alpha > 0){
                SetColor(0, 0, 0, _notificationText.color.a - fadeSpeed);
            }
        }

        pickupText.transform.position = Input.mousePosition;
        
    }


    
    

    public float GetPlayerToMouseRotation(){
        float ang = Mathf.Atan2(_cursorControl.GetMousePosition().y - transform.position.y, _cursorControl.GetMousePosition().x - transform.position.x) * Mathf.Rad2Deg;
        if (ang < 0){
            return 360 + ang;
        }
        return ang;
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

