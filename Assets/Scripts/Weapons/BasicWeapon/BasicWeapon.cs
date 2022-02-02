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
    


    protected Coroutine Coroutine;
    
    public bool looping;

    [SerializeField] private Vector2 offset = new Vector2(1.69f, -0.42f);
    [SerializeField] public int armType = 0;
    [SerializeField] private int cursorType = 0;
    [SerializeField] private bool audioRepeating = false;
    private CursorControl _cursorControl;


    protected override void Start(){
        base.Start();
        
        _cursorControl = FindObjectOfType<CursorControl>();
        Player.AddWeapon(new KeyValuePair<GameObject, KeyValuePair<BasicWeapon, Rigidbody2D>>(gameObject, new KeyValuePair<BasicWeapon, Rigidbody2D>(this, Body)));
    }

    
    
    /*
     * 
     * ================================================================================================================
     *                                               Pickup and Flipping
     * ================================================================================================================
     */

    protected virtual void FixedUpdate(){
        if (Player.primaryWeapon == this){
            if (Input.GetKey(KeyCode.Mouse0)){
                CheckFire();
            }
        }
    }

    public override void PickUp(Character character){
        base.PickUp(character);
        
        
        Player.WeaponImage.sprite = SpriteRenderer.sprite;
        _cursorControl.SetCursorType(cursorType);
        
        AudioManager.PlaySound(2, false, 0);
        
        transform.localPosition = offset;
        transform.localRotation = new Quaternion(0, 0, 0, 0);
        transform.localScale = new Vector3(Math.Abs(transform.localScale.x), Math.Abs(transform.localScale.y));
        Player.SetArmType(armType);
        RefreshText();
    }

    
    

    public override void Drop(){
        base.Drop();
        
    }
    
    
   
    
    /*
     * ================================================================================================================
     *                                               Virtuals
     * ================================================================================================================
     */
    
    
    protected virtual void Subtract(){
        AudioManager.PlaySound(0, audioRepeating, 0);
    }

    
    protected virtual void CheckFire(){
        
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
        SpriteRenderer.enabled = setEnabled;
    }

    protected virtual void Update(){
        
    }
    
    
}
