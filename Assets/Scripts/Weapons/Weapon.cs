using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Weapon : MonoBehaviour{
    

    
    [Header("Weapon")]
    
    
    
    [SerializeField] protected float pickupRange;
    [SerializeField] protected Character wielder;
    [SerializeField] public Vector2 offset = new Vector2(1.69f, -0.42f);

    public Rigidbody2D body;
    public SpriteRenderer spriteRenderer;

    
    public AudioManager AudioManager;
    
    protected PolygonCollider2D Collider;

    
    public virtual void PickUp(Character pickedUpBy, Transform parent){

        transform.parent = parent;
        wielder = pickedUpBy;
        body.simulated = false;
        spriteRenderer.sortingLayerID = pickedUpBy.spriteRenderer.sortingLayerID;
        spriteRenderer.sortingOrder = 4;
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

    protected virtual void Start(){
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Collider = GetComponent<PolygonCollider2D>();
        
        AudioManager = GetComponent<AudioManager>();    
    }
    
    protected virtual void Update(){
        
    }
    
    protected virtual void FixedUpdate(){
        
    }

    #endregion
    
    
}
