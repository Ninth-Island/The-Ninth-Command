using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : CustomObject{
    

    
    [Header("Weapon")]
    [SerializeField] protected float pickupRange;
    [SerializeField] protected Character wielder;

    


    public virtual void Ready(){
        
    }

    public void Pickup(Character character, Transform t){
        parent = t;
        wielder = character;
        body.simulated = false;
        
        spriteRenderer.sortingLayerID = character.spriteRenderer.sortingLayerID;
        spriteRenderer.sortingOrder = 4;

        Ready();
    }

    public virtual void Drop(){
        body.simulated = true;
        gameObject.layer = LayerMask.NameToLayer("Objects");
        transform.parent = null;

        spriteRenderer.sortingLayerID = SortingLayer.NameToID("Objects");
        spriteRenderer.sortingOrder = 0;
    }

    public void SetWielder(Character setWielder){
        wielder = setWielder;
    }

    public float GetPickupRange(){
        return pickupRange;
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
