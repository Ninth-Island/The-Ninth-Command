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
        base.CheckReload();
        if (Input.GetKey(KeyCode.R)){
            _cooling += coolingCooldownAcceleration;
            /*if (!AudioManager.soundsFromList[1].source.isPlaying){
                AudioManager.PlayFromList(1);
            }
        }
        if (_heat <= 0){
            AudioManager.soundsFromList[1].source.Stop();
            AudioManager.soundsFromList[4].source.Stop();
        }
        else{
            if (AudioManager.soundsFromList[1].source.time >= 2.6f){
                AudioManager.PlayFromList(4);
            }*/
        }
    }
    

    protected override void Subtract(){
        base.Subtract();
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
        Player.ammoCounter.SetText("");
        Player.magCounter.SetText("");
        Player.energyCounter.SetText("" + Mathf.RoundToInt(_energy));
        Player.heatCounter.SetText(Mathf.RoundToInt(_heat) + "% / 100%");
    }
    
    protected override void Start(){
        base.Start();
    }
}
