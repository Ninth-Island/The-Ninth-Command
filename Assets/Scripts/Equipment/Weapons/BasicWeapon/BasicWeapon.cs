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
    
    public Transform firingPoint;
    public bool activelyWielded = false;
    

    public virtual void Ready(){
        activelyWielded = true;
        AudioManager.PlaySound(2);
    }

    protected override void Pickup(Player player, int[] path){
        base.Pickup(player, path);
        Ready();
    }

    [Command]
    public void CmdAttemptFire(float angle){
        HandleFiring(angle);    
    }
    
    public virtual void HandleFiring(float angle){
        
    }

    
    protected virtual void HandleMagazineDecrement(){
        AudioManager.PlaySound(0);
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
        AudioManager.PlaySound(1);
    }


    public override void SwapTo(Player player, Equipment oldEquipment, int[] path){
        base.SwapTo(player, oldEquipment, path);
        
        spriteRenderer.sortingLayerID = player.spriteRenderer.sortingLayerID;
        spriteRenderer.sortingOrder = 4;

        player.primaryWeapon = this;

    }


    [Server]
    public override IEnumerator ServerInitializeEquipment(bool isThePrimaryWeapon, Player player, int[] path){
        wielder = player;
        netIdentity.AssignClientAuthority(player.connectionToClient);
        ClientSetWielder(player);

        yield return new WaitUntil(() => player.characterClientReady);
        CancelPickup(player, path);
        ClientInitializeEquipment(isThePrimaryWeapon, player, path);
    }

    [ClientRpc]
    protected override void ClientInitializeEquipment(bool isThePrimaryWeapon, Player player, int[] path){
        base.ClientInitializeEquipment(isThePrimaryWeapon, player, path);
        if (!isThePrimaryWeapon){
            activelyWielded = false;
            spriteRenderer.enabled = false;
        }
        
    }


    public override void Drop(){
        base.Drop();
        wielder = null;
        activelyWielded = false;
        spriteRenderer.enabled = true;
    }

}

