using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

public class BasicWeapon : Weapon{

    [SerializeField][Tooltip("Player Only")] public int armType = 0;
    [SerializeField][Tooltip("Player Only")] public int cursorType = 0;
    
    [SerializeField] private bool allowInterrupt = false;
    public Transform firingPoint;
    public bool activelyWielded = false;
    

    [Command]
    public void CmdAttemptFire(float angle){
        HandleFiring(angle);    
    }
    
    public virtual void HandleFiring(float angle){
        
    }

    
    protected virtual void HandleMagazineDecrement(){
        AudioManager.PlaySound(0, allowInterrupt);
    }


    public virtual void StopReloading(){
        StopAllCoroutines();
    }

    protected virtual void RefreshText(){
        
    }
    
     
    protected override void ClientUpdate(){
        base.ClientUpdate();
        if (activelyWielded){
            RefreshText();
        }
    }


    public virtual void Reload(){
        AudioManager.PlaySound(1, false);
    }


    public override void SwapTo(Character character, BasicWeapon oldWeapon, int[] path){
        base.SwapTo(character, oldWeapon, path);
        character.primaryWeapon = this;

    }

    public override void Ready(){
        activelyWielded = true;
        AudioManager.PlaySound(2, allowInterrupt);
    }

    [Server]
    public IEnumerator ServerInitializeWeapon(bool isThePrimaryWeapon, Character w, int[] path){
        wielder = w;
        netIdentity.AssignClientAuthority(w.connectionToClient);
        ClientSetWielder(w);

        yield return new WaitUntil(() => w.characterClientReady);
        CancelPickup(w, path);
        ClientInitializeWeapon(isThePrimaryWeapon, w, path);
    }

    [ClientRpc]
    private void ClientInitializeWeapon(bool isThePrimaryWeapon, Character w, int[] path){
        CancelPickup(w, path);
        if (!isThePrimaryWeapon){
            activelyWielded = false;
            spriteRenderer.enabled = false;
        }
    }

    [ClientRpc]
    private void ClientSetWielder(Character w){
        wielder = w;
    }


    protected override void Drop(){
        base.Drop();
        wielder = null;
        activelyWielded = false;
    }

}

