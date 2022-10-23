using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Weapon : CustomObject{
    

    
    [Header("Weapon")]
    [SerializeField] public Character wielder;

    private NetworkTransform _networkTransform;

    
    public virtual void SwapTo(Character character, BasicWeapon oldWeapon, int[] path){
        oldWeapon.Drop();
        Pickup(character, path);
        
        spriteRenderer.sortingLayerID = character.spriteRenderer.sortingLayerID;
        spriteRenderer.sortingOrder = 4;
    
    }

    public void CancelPickup(Character character, int[] path){
        Pickup(character, path);
        
        spriteRenderer.sortingLayerID = character.spriteRenderer.sortingLayerID;
        spriteRenderer.sortingOrder = 4;

    }

    private void Pickup(Character character, int[] path){
        parent = character.transform;
        _networkTransform.enabled = false;
        
        for (int i = 0; i < path.Length; i++){
            parent = parent.GetChild(path[i]);
        }
        wielder = character;
        body.simulated = false;
        Ready();

    }


    public virtual void Ready(){
    }
    

    protected virtual void Drop(){
        body.simulated = true;
        body.bodyType = RigidbodyType2D.Dynamic;
        gameObject.layer = LayerMask.NameToLayer("Objects");
        parent = null;
        _networkTransform.enabled = true;
        

        spriteRenderer.sortingLayerID = SortingLayer.NameToID("Objects");
        spriteRenderer.sortingOrder = 0;
    }
    


    #region Start Update

    protected override void Start(){
        base.Start();       
        _networkTransform = GetComponent<NetworkTransform>();

    }

    #endregion
    
    
}
