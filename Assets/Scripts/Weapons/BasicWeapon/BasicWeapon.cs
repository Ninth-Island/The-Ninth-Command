using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.SearchService;
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
    private CursorControl _cursorControl;

    protected override void Start(){
        base.Start();
        _cursorControl = FindObjectOfType<CursorControl>();
    }

    
    
    /*
     * 
     * ================================================================================================================
     *                                               Pickup and Flipping
     * ================================================================================================================
     */
    
    protected virtual void Update(){
        if (Player.primaryWeapon == this){
            if (Input.GetKey(KeyCode.Mouse0)){
                CheckFire();
            }
        }
    }


    private void OnMouseOver(){
        Player.pickupText.SetText("(G) " + name);

        if (Input.GetKeyDown(KeyCode.G)){
            RefreshText();

            base.PickUp(Player);
            Player.primaryWeapon.Drop();
            transform.localPosition = offset;
            transform.localRotation = new Quaternion(0f, 0f, 0f, 0);
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x), Math.Abs(transform.localScale.y));
            Player.primaryWeapon = this;
            Player.SetArmType(armType);
        }
    }

    protected override void Drop(){
        base.Drop();
        
    }
    
    
   
    
    /*
     * ================================================================================================================
     *                                               Virtuals
     * ================================================================================================================
     */
    
    
    protected virtual void Subtract(){
        AudioManager.PlayFromList(0);
    }

    
    protected virtual void CheckFire(){
        
    }
    public virtual void RefreshText(){
        Player.WeaponImage.sprite = SpriteRenderer.sprite;
        _cursorControl.SetCursorType(cursorType);
        
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
    
    
}
