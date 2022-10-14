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
    [SyncVar] public BasicWeapon primaryWeapon;

    public BasicWeapon primaryWeaponPrefab;

    [SyncVar] [SerializeField] protected int health;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float jumpVelocity = 18;
    
    [SerializeField] private PhysicsMaterial2D[] materials;

    protected int MaxHealth; // for healthbar and respawns
    protected BoxCollider2D FeetCollider; // for ground checks
    
    [SerializeField] protected Animator Animator;
    
    protected bool Airborne = true;
    protected bool FallingKnocked = false; // for falling too fast
    protected bool InputsFrozen = false; // for death
    

    private bool _suppressGroundCheck;

    public bool characterClientReady;


    // server keeps track of this and sends it to clients
    public List<ProjectileProperties> projectiles;

    
    #region Server

    
    [Server]
    protected override void ServerFixedUpdate(){
        base.ServerFixedUpdate();
        CheckStates();
    }
    
    // needed for client and server
    private void CheckStates(){ // happens on fixed update
        FallingKnocked = !(Math.Abs(body.velocity.x) < moveSpeed * 1.2); // if moving slow enough, return control to plr
        
        if (!_suppressGroundCheck){
            RaycastHit2D[] results = new RaycastHit2D[3];
            Physics2D.RaycastNonAlloc(transform.position, Vector2.down,  results, 4, LayerMask.GetMask("Ground", "Platform", "Vehicle", "Vehicle Outer", "Team 1", "Team 2", "Team 3", "Team 4"));
            
            Airborne = true;
            foreach (RaycastHit2D result in results){
                if (result && result.collider.gameObject != gameObject && result.collider.gameObject != FeetCollider.gameObject){
                    Airborne = false;
                }
            }
        }
    }

    
    protected IEnumerator ResetGroundCheck(){
        _suppressGroundCheck = true;
        yield return new WaitForSeconds(0.2f);
        _suppressGroundCheck = false;
    }




    #endregion


    #region Client

    
    protected override void Start(){
        base.Start();

        MaxHealth = health;
        FeetCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
        
    }
    
    [Client]
    protected override void ClientUpdate(){
        base.ClientUpdate();
    }

    protected override void ClientFixedUpdate(){
        base.ClientFixedUpdate();
        CheckStates();
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
        // the override is only a visual thing for the arm
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


    
    //UI
    
    [Client]
    public virtual void SetWeaponValues(int magazinesLeft, int magazineSize, int bulletsLeft, float energy, float heat, int type){
    }
    
    [Client]
    public virtual void SetReloadingText(string text){
    }
    

    public BoxCollider2D GetFeetCollider(){ // needed to ignore collisions
        return FeetCollider;
    }

    
    
    [ClientRpc]
    public virtual void HUDPickupWeapon(){ // called when smth changes like weapon swap
        
    }

    [Client]
    public override void OnStartClient(){
        base.OnStartClient();
        if (hasAuthority){
            CmdSetReady();
        }
    }

    [Command]
    private void CmdSetReady(){
        characterClientReady = true;
    }
    
    
}
