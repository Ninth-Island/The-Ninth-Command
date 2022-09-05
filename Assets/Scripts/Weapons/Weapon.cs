using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Weapon : CustomObject{
    

    
    [Header("Weapon")]
    [SerializeField] protected Character wielder;

    [Command]
    public void CmdReady(){
        ServerReady();
    }
   

    [Command]
    public void CmdPickup(Character character, int[] path){
        ServerPickup(character, path);
    }

    protected void ServerPickup(Character character, int[] path){
        Parent = character.transform;
        for (int i = 0; i < path.Length; i++){
            Parent = Parent.GetChild(path[i]);
        }
        wielder = character;
        body.simulated = false;
        
        spriteRenderer.sortingLayerID = character.spriteRenderer.sortingLayerID;
        spriteRenderer.sortingOrder = 4;

        ServerReady();
    }
 
    [Server]
    protected virtual void ServerReady(){
        
    }
    
    [Command]
    public virtual void CmdDrop(){
        body.simulated = true;
        gameObject.layer = LayerMask.NameToLayer("Objects");
        Parent = null;

        spriteRenderer.sortingLayerID = SortingLayer.NameToID("Objects");
        spriteRenderer.sortingOrder = 0;
    }
    


    #region Start Update

    public override void OnStartClient(){
        base.OnStartClient();
    }

    public override void OnStartServer(){
        base.OnStartServer();
    }

    protected override void Update(){
        base.Update();
    }
    
    protected override void FixedUpdate(){
        base.FixedUpdate();
    }

    #endregion
    
    
}
