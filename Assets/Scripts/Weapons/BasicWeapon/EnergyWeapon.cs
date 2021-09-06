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
    [SerializeField] private float defaultCooldownAcceleration;
    [SerializeField] private float coolingCooldownAcceleration;
    
    //max is 100
    public float _energy = 100;
    public float _heat = 0;
    public float _cooling = 0;
    
    
    /*
     * ================================================================================================================
     *                                               Firing Stuff
     * ================================================================================================================
     */
    
    protected override void CheckFire(){
        if (_energy > 0 && _heat < 100 && !Firing){
            StartCoroutine(Fire());
            _cooling = 0;
        }
    }


    /*
     * ================================================================================================================
     *                                               Cooling
     * ================================================================================================================
     */
    
    
    protected override void Update(){
        base.Update();
        _cooling += defaultCooldownAcceleration;
        _heat -= _cooling;
        if (_heat < 0){
            _heat = 0;
        }

        if (Player.primaryWeapon == this){
            RefreshText();
        }
    }
    
    public override void CheckReload(){
        if (Input.GetKey(KeyCode.R)){
            _cooling += coolingCooldownAcceleration;
            AudioManager.PlayFromList(1);
        }
    }
    

    protected override void Subtract(){
        _energy -= percentagePerShot;
        _heat += heatPerShot;
    }
    
    /*
    * ================================================================================================================
    *                                               Other
    * ================================================================================================================
    */
    
    public override void RefreshText(){
        base.RefreshText();
        PlayerPickupController.ammoCounter.SetText("");
        PlayerPickupController.magCounter.SetText("");
        PlayerPickupController.energyCounter.SetText("" + Mathf.RoundToInt(_energy));
        PlayerPickupController.heatCounter.SetText(Mathf.RoundToInt(_heat) + "% / 100%");
    }
    
    protected override void Start(){
        base.Start();
    }
}
