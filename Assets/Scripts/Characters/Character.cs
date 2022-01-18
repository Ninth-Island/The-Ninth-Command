﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour{
    
    /*
* ================================================================================================================
*                                               Character
*
*  The super parent of all players, enemies, allies
 *
 * contains protected variables such as health and faction (side), collider and rigid body
     *
     * Contains virtual method for taking damage and dying and such
*
* 
* ================================================================================================================
*/
    
    [SerializeField] protected int health;
    [SerializeField] protected float moveSpeed;
    
    [SerializeField] private PhysicsMaterial2D[] materials;


    protected BoxCollider2D Collider;
    
    protected BoxCollider2D FeetCollider;

    
    protected Rigidbody2D Body;
    protected Animator Animator;

    
    protected bool Airborne = true;
    protected bool FallingKnocked = false;
    protected bool InputsFrozen = false;

    protected AudioManager AudioManager;
    
    


    protected virtual void Start(){
        
        
        Collider = GetComponent<BoxCollider2D>();
        FeetCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();

        Body = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();

        AudioManager = GetComponent<AudioManager>();
        
        foreach (Weapon weapon in GetComponentsInChildren<Weapon>()){
            weapon.SetWielder(this);
        }
    }

    protected virtual void Update(){
        
    }
    protected virtual void FixedUpdate(){
        FallingKnocked = true;
        if (Math.Abs(Body.velocity.x) < moveSpeed * 1.2){
            FallingKnocked = false;
        }
        
        Airborne = true;
        RaycastHit2D clampScan = Physics2D.Raycast(transform.position, Vector2.down, 4,
            LayerMask.GetMask("Ground", "Platform", "Vehicle"));
        
        if (clampScan.collider){
            Airborne = false;
            
        }
    }

    public void Hit(Projectile projectile){
        TakeDamage(projectile.GetDamage());
    }

    protected virtual void TakeDamage(int damage){
        health -= damage;
        if (health <= 0){
            InputsFrozen = true;
            Destroy(gameObject, 3f);
        }
    }
    

    public Rigidbody2D GetBody(){
        return Body;
    }
    
    public BoxCollider2D GetCollider(){
        return Collider;
    }

    public void SetInputFrozen(bool setInputFrozenState, float unfreezeTime){
        InputsFrozen = setInputFrozenState;
        if (setInputFrozenState && unfreezeTime > 0){
            StartCoroutine(UnFreeze(unfreezeTime));
        }
    }

    IEnumerator UnFreeze(float time){
        yield return new WaitForSeconds(time);
        InputsFrozen = false;
    }
    
    public void SetKnocked(bool setKnockedState){
        FallingKnocked = setKnockedState; 
        // do some knocked stuff
        // rework animations;
    }
    
    protected virtual void OnCollisionEnter2D(Collision2D other){
        if (other.gameObject.CompareTag("Ground")){
            if (Math.Abs(Body.velocity.y) > 20){
                SortSound(4);
            }
            else{
                SortSound(3);
            }
        }
    }
    
    protected void SortSound(int type){

        PhysicsMaterial2D materialTouching = GetMaterialTouching();
        
        if (materialTouching != null){
            int soundIndex = 0;
            for (int i = 0; i < materials.Length; i++){
                PhysicsMaterial2D material = materials[i];
                if (material == materialTouching){
                    soundIndex = i * 5;
                }
            }

            soundIndex += type;
            AudioManager.PlaySound(soundIndex);
        }
    } 
    private PhysicsMaterial2D GetMaterialTouching(){
        if (FeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
            Collider2D[] output = new Collider2D[1];
            
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask("Ground", "Platform"));
            
            FeetCollider.OverlapCollider(filter, output);
            if (output[0] != null){
                return output[0].attachedRigidbody.sharedMaterial;
            }
        }

        return null;
    }

}
