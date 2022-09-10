using System;
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

    public bool activelyWielded = false;
    

    [Command]
    public void CmdAttemptFire(float angle){
        ServerHandleFiring(angle);    
    }
    
    [Server]
    public virtual void ServerHandleFiring(float angle){
        
    }

    
    [Server]
    protected virtual void HandleMagazineDecrement(){
        AudioManager.PlaySound(0, allowInterrupt);
    }
    
    [Client]
    public virtual void RefreshText(){
        
    }

    [Command]
    public virtual void CmdReload(){
        AudioManager.PlaySound(1, false);
    }
    



    [Server]
    protected override void ServerReady(){
        base.ServerReady();
        
        activelyWielded = true;
        RefreshText();
        AudioManager.PlaySound(2, allowInterrupt);
    }

    [Server]
    public IEnumerator ServerInitializeWeapon(bool isThePrimaryWeapon, Character w, int[] path){
        yield return new WaitUntil(() => NetworkClient.ready);
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

    [Command]
    public override void CmdDrop(){
        base.CmdDrop();
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
