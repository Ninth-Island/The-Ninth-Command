using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyWeapon : ProjectileWeapon{
    
    /*
    * ================================================================================================================
    *                                  Energy Weapon --> Basic Weapon --> Weapon
     *
     *   Heating and cooling system. Overheating stops firing. 
     * 
    * ================================================================================================================
    */
    
    [Header("Energy Weapon")] 
    [SerializeField] private float percentagePerShot;
    [SerializeField] private float heatPerShot;
    [SerializeField] private float coolDown;
    
    //max is 100
    public float _energy = 100;
    public float _heat = 0;

    private bool _isCooling;
    
    
    /*
     * ================================================================================================================
     *                                               Firing Stuff
     * ================================================================================================================
     */
    
    protected override void CheckFire(){
        if (_energy > 0 && _heat < 100 && !Firing && !_isCooling){
            StartCoroutine(Fire());
        }
    }


    /*
     * ================================================================================================================
     *                                               Cooling
     * ================================================================================================================
     */
    
    
    protected override void Update(){
        base.Update();
        RefreshText();
    }

    protected override void FixedUpdate(){
        base.FixedUpdate();


        if (_heat > 0){
            _heat -= coolDown;
        }

        if (_heat <= 0 && _isCooling){
            _heat = 0;

            _isCooling = false;
            AudioManager.PlaySound(2, false, 0);
        }
        
    }

    public override void CheckReload(){
        base.CheckReload();
        if (Input.GetKeyDown(KeyCode.R) && !_isCooling && _heat > 10){
            _isCooling = true;
            
            AudioManager.source.Stop();
            AudioManager.source.pitch = 1;
            AudioManager.PlaySound(1, true, 0);
        }
    }
    

    protected override void Subtract(){
        base.Subtract();
        
        AudioManager.source.pitch = 1 + (_heat / 75);
        _energy -= percentagePerShot;
        _heat += heatPerShot;

        if (_heat >= 100){
            _heat = 99.9f;
            AudioManager.source.Stop();
            AudioManager.source.pitch = 1;
            AudioManager.PlaySound(1, false, 0);
            _isCooling = true;
        }
    }
    
    
    /*
    * ================================================================================================================
    *                                               Other
    * ================================================================================================================
    */

    public override void RefreshText(){
        if (Player.primaryWeapon == this){
            base.RefreshText();
            Player.ammoCounter.SetText("");
            Player.magCounter.SetText("");
            
            Player.energyCounter.SetText("" + Mathf.RoundToInt(_energy));
            Player.heatCounter.SetText(Mathf.RoundToInt(_heat) + "% / 100%");
        }
    }

    protected override void Start(){
        base.Start();
    }
}
