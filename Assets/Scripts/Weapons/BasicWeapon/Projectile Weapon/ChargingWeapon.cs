using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

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

    public float heat = 0;
    public bool coolingDown;

    private bool _hasReleased = true;
    
    
    /*
     * ================================================================================================================
     *                                               Firing Stuff
     * ================================================================================================================
     */

    public override void HandleFiring(float angle){
        if (energy > 0 && heat < 100 && !coolingDown){
            heat += chargePerFrame;
            
            AudioManager.PlayConstant(4, false, heat / 100f);

            _hasReleased = false;
            if (heat >= 100){
                AudioManager.source.Stop();
                base.HandleFiring(angle);
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
        
        if (isServer && !coolingDown && !_hasReleased && Input.GetKeyUp(KeyCode.Mouse0)){
            AudioManager.source.Stop();
            AudioManager.PlaySound(5, false);
            _hasReleased = true;
        }
    }

    
    protected override void FixedUpdate(){

        base.FixedUpdate();
        
            heat -= cooldownSpeed;

            if (!coolingDown){ // so that after firing it cools down slower than before firing
                heat -= cooldownSpeed;
            }
            else{
                if (heat == 80){
                    AudioManager.PlaySound(1, false);
                }
            }

            if (heat <= 0){
                coolingDown = false;
                heat = 0;
                if (wielder){
                    wielder.FinishReload();
                }
            }
            
    }


    public override void Reload(){  
    }
    
    
    protected override void HandleMagazineDecrement(){
        base.HandleMagazineDecrement();
        energy -= percentagePerShot;
        heat = 100f / shotsPerSalvo;
        if (heat >= 100){
            coolingDown = true;
            wielder.Reload();
        }
    }
    
    /*
    * ================================================================================================================
    *                                               Other
    * ================================================================================================================
    */

    protected override void RefreshText(){
        wielder.SetWeaponValues(0, 0, 0, energy, heat, 3);
        
    }

    public override void OnStartClient(){
        base.OnStartClient();
    }
    
    protected override int GetSeed(){
        return (int)Math.Truncate(energy);
    }
}
