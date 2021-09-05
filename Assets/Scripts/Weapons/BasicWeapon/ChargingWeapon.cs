using System.Collections;
using System.Collections.Generic;
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
        base.Update();
        heat -= cooldownSpeed;
        if (!coolingDown){
            heat -= cooldownSpeed;
        }
        if (heat < 0){
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
        PlayerPickupController.ammoCounter.SetText("");
        PlayerPickupController.magCounter.SetText("");
        PlayerPickupController.energyCounter.SetText("" + Mathf.RoundToInt(energy));
        PlayerPickupController.heatCounter.SetText(Mathf.RoundToInt(heat) + "% / 100%");
    }
    
    protected override void Start(){
        base.Start();
    }
}
