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

    public override void CheckFire(float angle){
        if (_energy > 0 && _heat < 100 && !Firing && !_isCooling){
            StartCoroutine(Fire(angle));
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
            wielder.FinishReload();
        }
        
    }

    public override void CheckReload(){
        
        if (Input.GetKeyDown(KeyCode.R) && !_isCooling && _heat > 10){
            _isCooling = true;
            wielder.Reload();
            
            AudioManager.source.Stop();
            AudioManager.source.pitch = 1;
            base.CheckReload();
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
        if (wielder){
            wielder.SetWeaponValues(0, 0, 0, _energy, _heat, 2);
        }
    }

    protected override void Start(){
        base.Start();
    }
}
