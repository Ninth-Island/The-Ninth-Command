using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : CustomObject{
    

    
    [Header("Weapon")]
    [SerializeField] protected float pickupRange;
    [SerializeField] protected Character wielder;

    
    
    public AudioManager AudioManager;
    
    public SpriteRenderer spriteRenderer;
    protected PolygonCollider2D Collider;
    


    public virtual void PickUp(Character pickedUpBy){
        wielder = pickedUpBy;
        Body.simulated = false;
        spriteRenderer.sortingLayerID = pickedUpBy.spriteLayer;
        spriteRenderer.sortingOrder = 4;
        transform.parent = wielder.gameObject.transform.GetChild(1).transform.GetChild(5);
        // AudioManager.PlayFromList(2);
    }

    public virtual void Drop(){
        Body.simulated = true;
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

    protected override void Start(){
        base.Start();
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        Collider = GetComponent<PolygonCollider2D>();


        AudioManager = GetComponent<AudioManager>();
    }
    
    protected override void Update(){
        base.Update();
    }

    #endregion
    
    
}
