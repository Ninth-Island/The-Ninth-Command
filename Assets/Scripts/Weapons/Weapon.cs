using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : CustomObject{
    

    
    [Header("Weapon")]
    [SerializeField] protected float pickupRange;
    [SerializeField] protected Character wielder;

    
    
    public AudioManager AudioManager;
    
    protected PolygonCollider2D Collider;
    


    public virtual void PickUp(Character pickedUpBy){
        wielder = pickedUpBy;
        body.simulated = false;
        spriteRenderer.sortingLayerID = pickedUpBy.spriteRenderer.sortingLayerID;
        spriteRenderer.sortingOrder = 4;
        transform.parent = wielder.transform.GetChild(1).transform.GetChild(3);
        // AudioManager.PlayFromList(2);
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
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        Collider = GetComponent<PolygonCollider2D>();


        AudioManager = GetComponent<AudioManager>();    
    }

    public override void OnStartServer(){
        base.OnStartServer();
    }

    protected override void Update(){
        base.Update();
    }

    #endregion
    
    
}
