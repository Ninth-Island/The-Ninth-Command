using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class WeaponMod : CustomObject{


    public int maxCharge;
    public int chargeRate;
    public int chargeDrainPerFrame;
    [HideInInspector] public int currentAbilityCharge = 0;

    public BasicWeapon WeaponAttachedTo;
    protected bool Active;

    protected virtual void ModActiveFixedUpdate(){

    }

    public virtual void WeaponModFixedUpdate(){
        if (Active){
            ModActiveFixedUpdate();
            currentAbilityCharge -= chargeDrainPerFrame;
            if (currentAbilityCharge <= 0){
                Active = false;
                OutOfCharge();
            }
        }
        else{
            currentAbilityCharge = Mathf.Clamp(currentAbilityCharge + chargeRate, 0, maxCharge);
        }
    }

    public virtual void WeaponModInstant(){
        if (currentAbilityCharge >= maxCharge){
            Active = true;
            OverrideInstant();
        }
    }

    protected virtual void OutOfCharge(){
        
    }

    protected virtual void OverrideInstant(){
        
    }

    public virtual void WeaponModRelease(){
        
    }

    [Server]
    public void ServerAssignToWeapon(BasicWeapon weapon){
        AssignToWeaponClientRpc(weapon);
        AssignWeapon(weapon);
    }
    
    [ClientRpc]
    private void AssignToWeaponClientRpc(BasicWeapon weapon){
        AssignWeapon(weapon);}

    private void AssignWeapon(BasicWeapon weapon){
        weapon.weaponMod = this;
        WeaponAttachedTo = weapon;
        parent = weapon.transform;

    }


    
}
