using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Weapon : CustomObject{
    

    
    [Header("Weapon")]
    [SerializeField] public Character wielder;

    private NetworkTransform _networkTransform;

    [Command(requiresAuthority = false)]
    public void CmdPickup(Character character, BasicWeapon oldWeapon, int[] path){
        if (Vector2.Distance(character.transform.position, transform.position) <= 10){
            oldWeapon.Drop();
            ServerPickup(character, path);
            ServerAssignPrimaryWeapon(character);
        }
    }

    [Server]
    protected void ServerPickup(Character character, int[] path){
        netIdentity.AssignClientAuthority(character.connectionToClient);    
        
        Pickup(character, path);        
        ClientPickup(character.spriteRenderer.sortingLayerID, character, path);

        Ready();
    }

    private void Pickup(Character character, int[] path){
        parent = character.transform;
        if (isServer){ // temporary fix
            _networkTransform.enabled = false;
        }

        for (int i = 0; i < path.Length; i++){
            parent = parent.GetChild(path[i]);
        }
        wielder = character;
        body.simulated = false;

    }

    [ClientRpc]
    private void ClientPickup(int layer, Character setWielder, int[] path){
        Pickup(setWielder, path);
        spriteRenderer.sortingLayerID = layer;
        spriteRenderer.sortingOrder = 4;
    }

    [Server]
    protected virtual void ServerAssignPrimaryWeapon(Character character){
        
    }

    public virtual void Ready(){
    }
    
    [Server]
    protected virtual void Drop(){
        body.simulated = true;
        gameObject.layer = LayerMask.NameToLayer("Objects");
        parent = null;
        if (isServer){ // temporary fix
            _networkTransform.enabled = true;
        }

        spriteRenderer.sortingLayerID = SortingLayer.NameToID("Objects");
        spriteRenderer.sortingOrder = 0;
        netIdentity.RemoveClientAuthority();
    }
    


    #region Start Update

    public override void OnStartClient(){
        base.OnStartClient();
    }

    public override void OnStartServer(){
        base.OnStartServer();
        _networkTransform = GetComponent<NetworkTransform>();
    }

    protected override void Update(){
        base.Update();
    }
    
    protected override void FixedUpdate(){
        base.FixedUpdate();
    }

    #endregion
    
    
}
