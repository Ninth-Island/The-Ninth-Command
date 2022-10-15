﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

public class BasicWeapon : Weapon{

    [SerializeField] public Vector2 offset = new Vector2(1.69f, -0.42f); // this is gonna get deleted and replaced
    [SerializeField][Tooltip("Player Only")] public int armType = 0;
    [SerializeField][Tooltip("Player Only")] public int cursorType = 0;
    
    [SerializeField] private bool allowInterrupt = false;
    public Transform firingPoint;

    [SyncVar] public bool activelyWielded = false;
    

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

    [Server]
    protected override void ServerAssignPrimaryWeapon(Character character){
        character.primaryWeapon = this;
    }

    
    public override void Ready(){
        base.Ready();
        activelyWielded = true;
        if (isServer){ // temporary fix
            wielder.HUDPickupWeapon();
        }
        AudioManager.PlaySound(2, allowInterrupt);
    }

    [Server]
    public IEnumerator ServerInitializeWeapon(bool isThePrimaryWeapon, Character w, int[] path){
        yield return new WaitUntil(() => w.characterClientReady);
        ClientInitializeWeapon(isThePrimaryWeapon, w);
        ServerPickup(w, path);
    }

    [ClientRpc]
    private void ClientInitializeWeapon(bool isThePrimaryWeapon, Character w){
        
        if (!isThePrimaryWeapon){
            activelyWielded = false;
            spriteRenderer.enabled = false;
        }
    }

    [Server]
    protected override void Drop(){
        base.Drop();
        wielder = null;
        activelyWielded = false;
    }

    public override void OnStartClient(){
        base.OnStartClient();
    }

    protected override void Update(){
        base.Update();
    }
    
    protected override void FixedUpdate(){
        base.FixedUpdate();
    }

    private void OnDisable(){
        //AudioManager.source.Stop();
    }
    
}
