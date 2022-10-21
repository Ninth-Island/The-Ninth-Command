﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class Character : CustomObject{
    
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
    public BasicWeapon primaryWeapon;

    public BasicWeapon primaryWeaponPrefab;

    [SerializeField] protected int health;    
    [SerializeField] protected int shield;

    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float jumpVelocity = 18;
    
    [SerializeField] private PhysicsMaterial2D[] materials;

    protected int MaxHealth; // for healthbar and respawns
    protected int MaxShield; // for shieldBar
    [SerializeField] protected BoxCollider2D feetCollider; // for ground checks
    
    [SerializeField] protected Animator Animator;
    
    protected bool Airborne = true;
    protected bool FallingKnocked = false; // for falling too fast
    protected bool InputsFrozen = false; // for death
    

    private bool _suppressGroundCheck;

    public bool characterClientReady;

    public float dropoff = 0.95f;
    
    
    #region Server

    
    [Command]
    private void CmdSetReady(){
        characterClientReady = true;
    }
    
    
    #endregion


    #region Client
    
    
    [Client]
    protected void SortSound(int type){

        if (AudioManager){
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
                AudioManager.PlaySound(soundIndex, false);
            }
        }
    }

    [Client]
    private PhysicsMaterial2D GetMaterialTouching(){
        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
            Collider2D[] output = new Collider2D[1];
            
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask("Ground", "Platform"));
            
            feetCollider.OverlapCollider(filter, output);
            if (output[0] != null){
                return output[0].attachedRigidbody.sharedMaterial;
            }
        }
        return null;
    }


    
    //UI
    
    [Client]
    public virtual void SetWeaponValues(int magazinesLeft, int magazineSize, int bulletsLeft, float energy, float heat, int type){
    }
    
    [Client]
    public virtual void SetReloadingText(string text){
    }
 

    
    
    [Client]
    public virtual void HUDPickupWeapon(){ // called when smth changes like weapon swap
        
    }

    [Client]
    public override void OnStartClient(){
        if (hasAuthority){
            CmdSetReady();
        }
    }

    
    #endregion

    #region Shared

    protected override void Start(){
        base.Start();

        MaxHealth = health;
        MaxShield = shield;
        
    }
    
    protected override void FixedUpdate(){
        CheckStates();
        base.FixedUpdate(); // THIS HAS to be come first. The checks states has to set velocity BEFORE player does otherwise it nerds movement speed
    }


    private void CheckStates(){ // happens on fixed update
        FallingKnocked = !(Math.Abs(body.velocity.x) < moveSpeed * 1.2); // if moving slow enough, return control to plr

        if (!_suppressGroundCheck){
            RaycastHit2D[] resultsLeft = new RaycastHit2D[3];
            RaycastHit2D[] resultsRight = new RaycastHit2D[3];
            
            Physics2D.RaycastNonAlloc(transform.position - new Vector3(1.4f * transform.localScale.x, 0), Vector2.down,
                resultsLeft, 4,
                LayerMask.GetMask("Ground", "Platform", "Vehicle", "Vehicle Outer", "Team 1", "Team 2", "Team 3",
                    "Team 4"));

            Physics2D.RaycastNonAlloc(transform.position + new Vector3(0.73f * transform.localScale.x, 0), Vector2.down,
                resultsRight, 4,
                LayerMask.GetMask("Ground", "Platform", "Vehicle", "Vehicle Outer", "Team 1", "Team 2", "Team 3",
                    "Team 4"));
            
            Debug.DrawRay(transform.position + new Vector3(0.73f * transform.localScale.x, 0), Vector2.down * 4);
            Debug.DrawRay(transform.position - new Vector3(1.4f * transform.localScale.x, 0), Vector2.down * 4);
            
            Airborne = true;
            foreach (RaycastHit2D result in resultsLeft){
                if (result && result.collider.gameObject != gameObject &&
                    result.collider.gameObject != feetCollider.gameObject){
                    Airborne = false;
                    if (body.velocity.magnitude < 1f){
                        body.velocity *= dropoff;
                    }
                }
            }

            if (Airborne){
                foreach (RaycastHit2D result in resultsRight){
                    if (result && result.collider.gameObject != gameObject &&
                        result.collider.gameObject != feetCollider.gameObject){
                        Airborne = false;
    
                        if (body.velocity.magnitude < 1f){
                            body.velocity *= dropoff;
                        }
                    }

                }
            }
        }
    }

    protected IEnumerator ResetGroundCheck(){
        _suppressGroundCheck = true;
        yield return new WaitForSeconds(0.2f);
        _suppressGroundCheck = false;
    }


    [Server]
    protected virtual void Hit(int damage, Vector3 position, float angle){
    }



    public virtual void Reload(){ // called by weapon
        // the override is only a visual thing for the arm
    }


    public virtual void FinishReload(){ // same as above
        
    }
    
    protected virtual void OnCollisionEnter2D(Collision2D other){
        
    }
    
    public void SetLayer(int layer){
        gameObject.layer = layer;
        feetCollider.gameObject.layer = layer;
    }



    #endregion

    
}
