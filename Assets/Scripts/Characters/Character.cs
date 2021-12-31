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

    protected BoxCollider2D Collider;
    protected Rigidbody2D Body;
    protected Animator Animator;

    protected bool Airborne = true;
    protected bool FallingKnocked = false;
    protected bool InputsFrozen = false;

    protected AudioManager AudioManager;
    


    protected virtual void Start(){
        
        
        Collider = GetComponent<BoxCollider2D>();
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
            AudioManager.PlayFromList(2);
        }
    }

}
