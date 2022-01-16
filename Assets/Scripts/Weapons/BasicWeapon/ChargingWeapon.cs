using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ChargingWeapon : ProjectileWeapon
{
    
    /*
   * ================================================================================================================
   *                                  Charging Weapon --> Basic Weapon --> Weapon
     *
     *  Weapon that works very similarly to energy weapon. Charges heat until heat is at 100, then fires and cools down
     * 
   * ================================================================================================================
   */
    
    
    [Header("Charging Weapon")]
    [SerializeField] private float chargePerFrame;
    [SerializeField] private float cooldownSpeed;

    [SerializeField] float percentagePerShot;

    [SerializeField] protected float energy;

    [SerializeField] private float chargeSoundLengthDivider;
    public float heat = 0;
    public bool coolingDown;
    
    
    /*
     * ================================================================================================================
     *                                               Firing Stuff
     * ================================================================================================================
     */
    
    protected override void CheckFire(){
        if (energy > 0 && heat < 100 && !Firing && !coolingDown){
            heat += chargePerFrame;
            
        //    AudioManager.sounds[0].source.mute = false;        
          //  AudioManager.sounds[1].source.Stop();
            
            //AudioManager.sounds[0].source.time = heat / chargeSoundLengthDivider;
            //AudioManager.sounds[0].source.Play();
            //if (!AudioManager.sounds[0].source.isPlaying){ // currently being actively charged so charge it
                
            //}
            if (heat >= 100){
                StartCoroutine(Fire());
            }
        }
        
    }


    /*
     * ================================================================================================================
     *                                               Cooling
     * ================================================================================================================
     */
    
    
    protected override void Update(){
//        AudioManager.sounds[0].source.mute = true;
        base.Update();
        
        heat -= cooldownSpeed;
        if (!coolingDown){ // so that after firing it cools down slower than before firing
            heat -= cooldownSpeed;
           // if (heat > 0 && !AudioManager.sounds[1].source.isPlaying && !AudioManager.sounds[0].source.isPlaying){
             //   AudioManager.sounds[1].source.Play();
           // }

        }
        if (heat <= 0){
            coolingDown = false;
            heat = 0;
        }
        

        if (Player.primaryWeapon == this){
            RefreshText();
        }
    }
    
    public override void CheckReload(){
        base.CheckReload();
    }
    

    protected override void Subtract(){
        base.Subtract();
        energy -= percentagePerShot;
        heat = 100;
        coolingDown = true;
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
        Player.energyCounter.SetText("" + Mathf.RoundToInt(energy));
        Player.heatCounter.SetText(Mathf.RoundToInt(heat) + "% / 100%");
    }
    
    protected override void Start(){
        base.Start();
    }
}
