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

    [Server]
    protected void ServerPickup(Character character, int[] path){
        parent = character.transform;
        for (int i = 0; i < path.Length; i++){
            parent = parent.GetChild(path[i]);
        }
        wielder = character;
        body.simulated = false;
        
        SetSpriteLayer(character.spriteRenderer.sortingLayerID);

        ServerReady();
    }

    [ClientRpc]
    private void SetSpriteLayer(int layer){
        spriteRenderer.sortingLayerID = layer;
        spriteRenderer.sortingOrder = 4;
    }
 
    [Server]
    protected virtual void ServerReady(){
        
    }
    
    [Command]
    public virtual void CmdDrop(){
        body.simulated = true;
        gameObject.layer = LayerMask.NameToLayer("Objects");
        parent = null;

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
