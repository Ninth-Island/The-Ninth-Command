using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : CustomObject{
    
    /*
    * ================================================================================================================
    *                                               Weapon
     *
     *  The super parent of all weapons. Guns, Grenades, Power Ups, whatever. Contains basic information for picking up
     * and faction (side) for damage dealing and support
     *
     * Contains variables
     *
     * 
    * ================================================================================================================
    */
    
    [Header("Weapon")]
    [SerializeField] protected float pickupRange;
    [SerializeField] protected Character wielder;

    public AudioManager AudioManager;
    
    public SpriteRenderer spriteRenderer;
    protected PolygonCollider2D Collider;


    
    // Start is called before the first frame update
    protected override void Start(){
        base.Start();
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        Collider = GetComponent<PolygonCollider2D>();


        AudioManager = GetComponent<AudioManager>();
    }

    // Update is called once per frame
    protected override void Update(){
        base.Update();
    }

    public float GetPickupRange(){
        return pickupRange;
    }

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
    
}
