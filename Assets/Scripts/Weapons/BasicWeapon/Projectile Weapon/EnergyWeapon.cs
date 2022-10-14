using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Telepathy;
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



    public override void HandleFiring(float angle){
        if (!_isCooling){
            if (_energy >= percentagePerShot){
                base.HandleFiring(angle);
            }
        }
    }

    
    
    protected override void FixedUpdate(){
        base.FixedUpdate();


        if (_heat > 0){
            _heat -= coolDown;
        }

        if (_heat <= 0 && _isCooling){
            _heat = 0;

            _isCooling = false;
            AudioManager.PlaySound(2, false);
            wielder.FinishReload();
        }
        
    }

    public override void Reload(){
        
        if (!_isCooling && _heat > 10){
            _isCooling = true;
            wielder.Reload();
            
            AudioManager.source.Stop();
            AudioManager.source.pitch = 1;
            base.Reload();
        }
    }

    [Server]
    public override void StopReloading(){
        base.StopReloading();
        _isCooling = false;
    }

    
    protected override void HandleMagazineDecrement(){
        base.HandleMagazineDecrement();
        
        AudioManager.source.pitch = 1 + (_heat / 75);
        _energy -= percentagePerShot;
        _heat += heatPerShot;

        if (_heat >= 100){
            _heat = 99.9f;
            AudioManager.source.Stop();
            AudioManager.source.pitch = 1;
            AudioManager.PlaySound(1, false);
            _isCooling = true;
        }
    }


    protected override void RefreshText(){
        wielder.SetWeaponValues(0, 0, 0, _energy, _heat, 2);
        
    }

    public override void OnStartClient(){
        base.OnStartClient();
    }
}
