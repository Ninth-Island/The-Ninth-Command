using System;
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
    
    [SyncVar] [SerializeField] protected int health;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float jumpVelocity = 18;
    
    [SerializeField] private PhysicsMaterial2D[] materials;

    protected int MaxHealth; // for healthbar and respawns
    private BoxCollider2D _feetCollider; // for ground checks
    
    protected Animator Animator;
    
    protected bool Airborne = true;
    protected bool FallingKnocked = false; // for falling too fast
    protected bool InputsFrozen = false; // for death
    
    protected float XMove;

    private bool _suppressGroundCheck;


    
    #region Server

    
    [ServerCallback]
    protected override void FixedUpdate(){
        base.FixedUpdate();
        ServerMove();
        CheckStates();
        
    }
    
    
    [Command]
    protected void CmdSetXMoveServer(float xMove){
        XMove = xMove;
    }

    [Server]
    protected virtual void ServerMove(){ // happens on fixed update
        XMove = Mathf.Clamp(XMove, -1, 1);
        if (XMove != 0 && !InputsFrozen && !FallingKnocked){
            body.velocity = new Vector2(moveSpeed * XMove, body.velocity.y);
        }
    }

    [Command]
    protected virtual void CmdServerJump(){ // happens only as called
        
        Vector2 velocity = body.velocity;
        if (_feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground", "Platform", "Vehicle", "Vehicle Outer"))){
            Airborne = true;
            body.velocity = new Vector2(velocity.x, velocity.y + jumpVelocity);
            SortSound(0);
        }
    }

    [Server]
    private void CheckStates(){ // happens on fixed update
        FallingKnocked = !(Math.Abs(body.velocity.x) < moveSpeed * 1.2); // if moving slow enough, return control to plr
        
        if (!_suppressGroundCheck){
            RaycastHit2D groundCheck = Physics2D.Raycast(transform.position, Vector2.down, 4, LayerMask.GetMask("Ground", "Platform", "Vehicle"));

            Airborne = !groundCheck.collider; // if the raycast hit something set to not airborne and vice versa
        }
    }

    [Command]
    protected void CmdSetSuppressGroundCheck(){
        /*
        the player has a little bit of area where they're not technically touching the ground but need to act like it 
        for the game to look good like when going down a slope. Because of this, right after jumping the player is still
        inside this area, so the ground check needs to be temporarily suppresses right after jumping
        */ 
        StartCoroutine(ResetGroundCheck());
    }

    [Server]
    private IEnumerator ResetGroundCheck(){
        _suppressGroundCheck = true;
        yield return new WaitForSeconds(0.2f);
        _suppressGroundCheck = false;
    }

    [Command]
    protected void CmdAnimatorSetBool(string state, bool setTo){
        Animator.SetBool(state, setTo);
    }
    
    
    

    #endregion


    #region Client

    
    public override void OnStartClient(){
        base.OnStartClient();
        
        Animator = GetComponent<Animator>();
        AudioManager = GetComponent<AudioManager>();
        
        MaxHealth = health;
        _feetCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
        
    }

    
    [ClientCallback]
    protected override void Update(){
        /*Client calls command on server when jump key is pressed (checked for in update)*/
        base.Update();
        if (hasAuthority){
            ClientHandleMove();
            ClientHandleJump();
        }
    }


    [Client]
    protected virtual void ClientHandleMove(){
        
    }

    [Client]
    protected virtual void ClientHandleJump(){
        
    }

    
    #endregion

    

    [Server]
    public virtual void Hit(int damage){
        health -= damage;
        if (health <= 0){
            InputsFrozen = true;
            Destroy(gameObject);
        }
    }

    [Client]
    public virtual void Reload(){ // called by weapon
        // the override is only a visual thing
    }

    [Client]
    public virtual void FinishReload(){ // same as above
        
    }
    
    protected virtual void OnCollisionEnter2D(Collision2D other){
        // just for sounds
    }

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
        if (_feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
            Collider2D[] output = new Collider2D[1];
            
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask("Ground", "Platform"));
            
            _feetCollider.OverlapCollider(filter, output);
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
    
    [Server]
    public BoxCollider2D GetFeetCollider(){ // needed to ignore collisions
        return _feetCollider;
    }

}
