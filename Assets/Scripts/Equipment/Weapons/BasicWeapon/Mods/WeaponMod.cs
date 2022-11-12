using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class WeaponMod : CustomObject{

    public BasicWeapon WeaponAttachedTo;
    protected bool Active;

    protected virtual void ModActiveFixedUpdate(){
        
    }
    
    public virtual void WeaponModFixedUpdate(){
        if (Active){
            ModActiveFixedUpdate();
        }
    }

    public virtual void WeaponModInstant(){
        
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
