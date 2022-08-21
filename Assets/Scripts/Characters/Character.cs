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
    
    [SerializeField] protected int health;
    [SerializeField] public float moveSpeed;
    [SerializeField] protected float jumpPower = 18;

    
    [SerializeField] private PhysicsMaterial2D[] materials;

    protected int maxhealth;


    protected BoxCollider2D Collider;
    
    protected BoxCollider2D FeetCollider;

    
    protected Animator Animator;

    
    protected bool Airborne = true;
    protected bool FallingKnocked = false;
    protected bool InputsFrozen = false;

    protected AudioManager AudioManager;
    
    protected float XMove;

    protected bool SuppressGroundCheck;


    #region Server

    [Command]
    protected virtual void CmdServerUpdate(){ // as soon as get here XMove is 0 for client
        ServerMove();
        CheckStates();
    
    }

    [Command]
    protected void CmdSetXMoveServer(float xMove){
        XMove = xMove;
    }

    [Server]
    protected virtual void ServerMove(){
        XMove = Mathf.Clamp(XMove, -1, 1);
        if (XMove != 0 && !InputsFrozen && !FallingKnocked){
            body.velocity = new Vector2(moveSpeed * XMove, body.velocity.y);
        }
    }
    
    
    
    [Command]
    protected virtual void CmdServerJump(){
        
        Vector2 velocity = body.velocity;
        
        // can't use airborne because the player is considered not airborne a few seconds before and after jumping
        if (FeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground", "Platform", "Vehicle", "Vehicle Outer"))){
            Airborne = true;
            body.velocity = new Vector2(velocity.x, velocity.y + jumpPower);
            
            SortSound(0);
        }
    
    }


    [Server]
    private void CheckStates(){
        if (Math.Abs(body.velocity.x) < moveSpeed * 1.2){
            FallingKnocked = false;
        }
        else{
            FallingKnocked = true;
        }

        if (!SuppressGroundCheck){
            RaycastHit2D clampScan = Physics2D.Raycast(transform.position, Vector2.down, 4,
                LayerMask.GetMask("Ground", "Platform", "Vehicle"));

            if (clampScan.collider){
                Airborne = false;
            }
            else{
                Airborne = true;
            }
        }
    }

    [Command]
    protected void CmdSetSuppressGroundCheck(){
        StartCoroutine(ResetGroundCheck());
    }

    [Server]
    private IEnumerator ResetGroundCheck(){
        SuppressGroundCheck = true;
        yield return new WaitForSeconds(0.2f);
        SuppressGroundCheck = false;
    }

    [Command]
    protected void CmdAnimatorSetBool(string state, bool setTo){
        Animator.SetBool(state, setTo);
    }
    
    

    #endregion


    #region Client

    
    public override void OnStartClient(){
        base.OnStartClient();

        maxhealth = health;
        
        Collider = GetComponent<BoxCollider2D>();
        FeetCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();

        Animator = GetComponent<Animator>();

        AudioManager = GetComponent<AudioManager>();
        
        foreach (Weapon weapon in GetComponentsInChildren<Weapon>()){
            weapon.SetWielder(this);
        }
    }

    [ClientCallback]
    protected override void Update(){
        base.Update();
        if (hasAuthority){
            ClientJump();
        }
    }


    [ClientCallback]
    protected override void FixedUpdate(){
        base.FixedUpdate();
        if (hasAuthority){

            ClientMove();
            CmdServerUpdate();

        }
    }

    [Client]
    protected virtual void ClientMove(){
        
    }

    [Client]
    protected virtual void ClientJump(){
        
    }

    
    #endregion

    
    
    
    
    
    
    

    public void Hit(Projectile projectile){
        TakeDamage(projectile.GetDamage());
    }

    protected virtual void TakeDamage(int damage){
        health -= damage;
        if (health <= 0){
            InputsFrozen = true;
            Destroy(gameObject);
        }
    }
    

    public Rigidbody2D GetBody(){
        return body;
    }
    
    public BoxCollider2D GetCollider(){
        return Collider;
    }
    
    public BoxCollider2D GetFeetCollider(){
        return FeetCollider;
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

    public virtual void Reload(){
        
    }

    public virtual void FinishReload(){
        
    }
    
    protected virtual void OnCollisionEnter2D(Collision2D other){
        
    }

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

    
    public virtual void SetWeaponValues(int magazinesLeft, int magazineSize, int bulletsLeft, float energy, float heat, int type){

    }

    public virtual void SetReloadingText(string text){
        
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
