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
    [SerializeField] private float earlyCutoff;

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
            AudioManager.source.Stop();
            
            AudioManager.PlaySound(6, true, (AudioManager.sounds[6].clipsList[0].length - earlyCutoff) / 105 * heat);
           
            
            if (heat >= 100){
                AudioManager.PlaySound(1, false, 0);
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
        base.Update();
        
        if (Input.GetKeyUp(KeyCode.Mouse0) && Player.primaryWeapon == this && !coolingDown){
            AudioManager.source.Stop();
            AudioManager.PlaySound(7, false, 0);
        }
        RefreshText();
    }

    protected override void FixedUpdate(){

        base.FixedUpdate();
        
        if (Player.primaryWeapon == this){
            

            if (Input.GetKey(KeyCode.Mouse0)){
                CheckFire();
            }

            
            heat -= cooldownSpeed;

            if (!coolingDown){ // so that after firing it cools down slower than before firing
                heat -= cooldownSpeed;


            }

            if (heat <= 0){
                coolingDown = false;
                heat = 0;
            }


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
        if (Player.primaryWeapon == this){
            base.RefreshText();
            Player.ammoCounter.SetText("");
            Player.magCounter.SetText("");
            
            Player.energyCounter.SetText("" + Mathf.RoundToInt(energy));
            Player.heatCounter.SetText(Mathf.RoundToInt(heat) + "% / 100%");
        }
    }

    protected override void Start(){
        base.Start();
    }
}
