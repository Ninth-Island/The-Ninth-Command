using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using UnityEngine;

public class BasicWeapon : Weapon{
    
    /*
   * ================================================================================================================
   *                                  Basic Weapon --> Weapon
     *
     * Any weapon that can be held in the hands. Energy Sword, Gravity Hammer, Guns, Lasers. Primary and Secondary
     *
     * Contains logic for picking up and some virtuals called by player
     *
     * 
   * ================================================================================================================
   */
    
    
    // major code cleanup of all projectiles weapons and characters due soon
    


    protected Coroutine Coroutine;
    
    public bool looping;

    [SerializeField] public Vector2 offset = new Vector2(1.69f, -0.42f);
    [SerializeField] public int armType = 0;
    [SerializeField] public int cursorType = 0;
    [SerializeField] private bool audioRepeating = false;
    protected CursorControl CursorControl;


    protected override void Start(){
        base.Start();
    
        CursorControl = FindObjectOfType<CursorControl>();

        foreach (Player player in FindObjectsOfType<Player>()){
            player.AddWeapon(new KeyValuePair<GameObject, KeyValuePair<BasicWeapon, Rigidbody2D>>(gameObject, new KeyValuePair<BasicWeapon, Rigidbody2D>(this, Body)));

        }
    }

    
    
    /*
     * 
     * ================================================================================================================
     *                                               Pickup and Flipping
     * ================================================================================================================
     */

    protected override void FixedUpdate(){
    }

    public override void PickUp(Character character){
        base.PickUp(character);

        transform.localRotation = new Quaternion(0, 0, 0, 0);
        transform.localScale = new Vector3(Math.Abs(transform.localScale.x), Math.Abs(transform.localScale.y));
        RefreshText();
    }

    
    

    public override void Drop(){
        base.Drop();
        wielder = null;
    }
    
    
   
    
    /*
     * ================================================================================================================
     *                                               Virtuals
     * ================================================================================================================
     */
    
    
    protected virtual void Subtract(){
        AudioManager.PlaySound(0, audioRepeating, 0);
    }


    public virtual void CheckFire(float angle){
        
    }

    
    public virtual void RefreshText(){
        
    }
    
    /*
     * ================================================================================================================
     *                                               Other
     * ================================================================================================================
     */

    public virtual void CheckReload(){
        
    }

    public virtual void SetLoadingState(){
        
    }
    
    public void SetSpriteRenderer(bool setEnabled){
        spriteRenderer.enabled = setEnabled;
    }

    protected override void Update(){
        base.Update();
    }
    
    
}
