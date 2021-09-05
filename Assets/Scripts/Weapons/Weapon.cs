﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour{
    
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
    [SerializeField] private float pickupRange;
    [SerializeField] protected Character wielder;
    
    
    
    protected SpriteRenderer SpriteRenderer;
    protected PolygonCollider2D Collider;
    protected Rigidbody2D Body;

    protected Player Player;
    protected WeaponPickup PlayerPickupController;
    
    // Start is called before the first frame update
    protected virtual void Start(){
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Collider = GetComponent<PolygonCollider2D>();
        Body = GetComponent<Rigidbody2D>();

        Player = FindObjectOfType<Player>();
        PlayerPickupController = FindObjectOfType<WeaponPickup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetPickupRange(){
        return pickupRange;
    }

    public virtual void PickUp(Character pickedUpBy){
        wielder = pickedUpBy;
        Body.simulated = false;
        transform.parent = Player.gameObject.transform;
            PlayerPickupController.SetText("Picked Up " + name);
    }

    protected virtual void Drop(){
        Body.simulated = true;
        gameObject.layer = LayerMask.NameToLayer("Objects");
        transform.parent = null;
    }

    public void SetWielder(Character setWielder){
        wielder = setWielder;
    }
    
}
