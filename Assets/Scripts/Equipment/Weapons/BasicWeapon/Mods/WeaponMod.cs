using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class WeaponMod : CustomObject{


    public int maxCharge;
    public int chargeRate;
    public int chargeDrainPerFrame;
    [SerializeField] private AudioSource source;
    [HideInInspector] public int currentAbilityCharge = 0;

    public BasicWeapon WeaponAttachedTo;
    protected bool Active;

    protected override void Start(){
        base.Start();
        Actions = new Action[4];
        Actions[0] = ModActiveFixedUpdate;
        Actions[1] = OutOfCharge;
        Actions[2] = OverrideInstant;
        Actions[3] = PlaySound;
    }


    protected virtual void ModActiveFixedUpdate(){ // this gets run on client and server once per frame each if active

    }

    public virtual void WeaponModFixedUpdate(){
        if (Active){
            RunOnBoth(0);
            currentAbilityCharge -= chargeDrainPerFrame;
            if (currentAbilityCharge <= 0){
                Active = false;
                RunOnBoth(1);
            }
        }
        else{
            currentAbilityCharge = Mathf.Clamp(currentAbilityCharge + chargeRate, 0, maxCharge);
        }
    }

    public void WeaponModInstant(){
        if (currentAbilityCharge >= maxCharge){
            Active = true;
            RunOnBoth(2);
            RunOnBoth(3);
        }
    }

    protected virtual void OutOfCharge(){
        
    }

    protected virtual void OverrideInstant(){ // this gets run on client and server once each if enough energy
        
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

    private void PlaySound(){
        source.Stop();
        source.Play();
    }





}
