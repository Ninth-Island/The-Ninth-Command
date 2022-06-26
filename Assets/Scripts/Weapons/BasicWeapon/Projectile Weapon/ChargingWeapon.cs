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

    private bool hasReleased = true;
    
    
    /*
     * ================================================================================================================
     *                                               Firing Stuff
     * ================================================================================================================
     */

    public override void AttemptFire(float angle){
        if (energy > 0 && heat < 100 && !coolingDown){
            heat += chargePerFrame;
            AudioManager.source.Stop();
            
            AudioManager.PlaySound(4, true, (AudioManager.sounds[4].clipsList[0].length - earlyCutoff) / 105 * heat);

            hasReleased = false;
            if (heat >= 100){
                base.AttemptFire(angle);
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
        
        if (!coolingDown && !hasReleased && Input.GetKeyUp(KeyCode.Mouse0)){
            AudioManager.source.Stop();
            AudioManager.PlaySound(5, false, 0);
            hasReleased = true;
        }
        RefreshText();
    }

    protected override void FixedUpdate(){

        base.FixedUpdate();
        
            heat -= cooldownSpeed;

            if (!coolingDown){ // so that after firing it cools down slower than before firing
                heat -= cooldownSpeed;
            }
            else{
                if (heat == 80){
                    AudioManager.PlaySound(1, false, 0);
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
        heat = 100;
        coolingDown = true;
        wielder.Reload();
    }
    
    /*
    * ================================================================================================================
    *                                               Other
    * ================================================================================================================
    */

    public override void RefreshText(){
        if (wielder){
            wielder.SetWeaponValues(0, 0, 0, energy, heat, 3);
        }
    }

    protected override void Start(){
        base.Start();
    }
}
