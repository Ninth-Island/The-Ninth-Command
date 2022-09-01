using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.XR;

public partial class Player : Character{
    
    /*
* ================================================================================================================
*                                               Player
*
 * contains all player logic. Movement, weapons, grenades, swapping weapons, animation, sounds, and additional foot collider
*
* 
* ================================================================================================================
*/

    [Header("Basic Weapons")] 
    public BasicWeapon primaryWeapon;
    public BasicWeapon secondaryWeapon;
    
    public BasicWeapon primaryWeaponPrefab;
    public BasicWeapon secondaryWeaponPrefab;

    private bool _isCrouching;

    private bool hardLanding;

    private bool _swappedWeapon;

    
    #region Server

    [Server]
    protected override void ServerMove(){

        XMove = Mathf.Clamp(XMove, -1, 1);
        Animator.SetBool(_aNames.running, XMove != 0);

        if (XMove != 0 && !InputsFrozen && !FallingKnocked){

            Animator.SetBool(_aNames.runningBackwards, Math.Sign(XMove) != Math.Sign(transform.localScale.x));
            
            if (_isCrouching){
                body.velocity = new Vector2(moveSpeed / 2 * XMove, body.velocity.y);
                SortSound(2);
            }
            else{
                body.velocity = new Vector2(moveSpeed * XMove, body.velocity.y);
            }
            
        }
    }


    [Command]
    protected override void CmdServerJump(){
        base.CmdServerJump();
        Animator.SetBool(_aNames.jumping, true);
    }

    [Command]
    protected override void CmdAnimatorUpdateAirborne(){
        Animator.SetBool(_aNames.jumping, Airborne);
    }



    #endregion

    #region Client


    
    [Client]
    protected override void ClientMove(){
        CmdSetXMoveServer(Input.GetAxis("Horizontal"));
    }

    [Client]
    protected override void ClientJump(){
        if (Input.GetKeyDown(KeyCode.W)){
            CmdServerJump();
            CmdSetSuppressGroundCheck();
        }
    
    }

    

    /*
    * ================================================================================================================
    *                                               Start and update
    * ================================================================================================================
    */

    public override void OnStartClient(){
        base.OnStartClient();

        HUDVisualStart();
        ControlStart();

        if (hasAuthority){
            _virtualCamera[0].Priority = 10;
        }
    }


    [ClientCallback]
    protected override void FixedUpdate(){
        base.FixedUpdate();

        /*if (Input.GetKey(KeyCode.Mouse0)){
            primaryWeapon.AttemptFire(GetBarrelToMouseRotation() * Mathf.Deg2Rad);
        }*/
        if (hasAuthority){
            ControlFixedUpdate();
        }

        if (Math.Abs(body.velocity.x) > 20 || Math.Abs(body.velocity.y) > 70){
            hardLanding = true;
        }
        else{
            hardLanding = false;
        }
        
    }

    [ClientCallback]
    protected override void Update(){
        base.Update();

        if (hasAuthority){
            CheckSwap();
            CheckCrouch();

            CmdAnimatorUpdateAirborne();
            if (Input.GetKey(KeyCode.R)){
                primaryWeapon.Reload();
            }
            
            CmdRotateArm(GetBarrelToMouseRotation());

            ControlUpdate();
            CheckForPickup();
        }
        
    }

    /*
    * ================================================================================================================
    *                                               Movement
    * ================================================================================================================
    */

    


    [Client]
    private void CheckCrouch(){
        if (Input.GetKeyDown(KeyCode.S)){
            _isCrouching = !_isCrouching;
            CmdAnimatorSetBool(_aNames.crouching, _isCrouching);
        }
    }


    #endregion

    #region Weapon and Vehicle
    /*
    * ================================================================================================================
    *                                               Weapon and Vehicle
    * ================================================================================================================
    */
    
    private void CheckSwap(){

        if (Math.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0 && !Input.GetKey(KeyCode.Mouse1) && !_swappedWeapon){
            if (primaryWeapon != null){
                primaryWeapon.SetSpriteRenderer(false);
            }

            if (secondaryWeapon != null){
                secondaryWeapon.SetSpriteRenderer(true);
            }

            primaryWeapon.Ready();

            (primaryWeapon, secondaryWeapon) = (secondaryWeapon, primaryWeapon);

            secondaryWeapon.activelyWielded = false;
            secondaryWeapon.gameObject.SetActive(false);
            
            primaryWeapon.activelyWielded = true;
            primaryWeapon.gameObject.SetActive(true);
            primaryWeapon.PickUp(this, arm);
            UpdateHUD();

            _swappedWeapon = true;
            Invoke(nameof(ResetSwappedWeapon), 0.25f);
        }
    }

    private void ResetSwappedWeapon(){
        _swappedWeapon = false;
    }
    
    private void CheckForPickup(){
        Vector2 mousePos = _cursorControl.GetMousePosition();
        RaycastHit2D objectScan =
            Physics2D.CircleCast(mousePos, 1, new Vector2(), 0, LayerMask.GetMask("Objects", "Vehicle"));
        
        pickupText.SetText("");
        if (objectScan){
            GameObject nearestObject = objectScan.collider.gameObject;
            pickupText.transform.localPosition = Input.mousePosition - HUD.transform.localPosition;
            pickupText.SetText(nearestObject.name);

            if (nearestObject.CompareTag("Weapon")){
                WeaponPickup(nearestObject);
            }

            if (nearestObject.CompareTag("Vehicle")){
                VehicleEmbark(nearestObject);
            }
        }
    }

    private void WeaponPickup(GameObject nearestObject){
        BasicWeapon weapon = _allBasicWeapons[nearestObject].Key;

        if (IsTouching(transform.position, nearestObject.transform.position,
            weapon.GetPickupRange(), weapon.GetPickupRange())){
            pickupText.SetText("(G) " + weapon.name);
            if (Input.GetKeyDown(KeyCode.G)){
                primaryWeapon.Drop();
                
                primaryWeapon = weapon;
                weapon.PickUp(this, arm); // some of this is redundant, rework
                UpdateHUD();
            }
        }
    
    }

    public override void SetWeaponValues(int magazinesLeft, int magazineSize, int bulletsLeft, float energy, float heat, int type){
        base.SetWeaponValues(magazinesLeft, magazineSize, bulletsLeft, energy, heat, type);

        if (type == 1){ // bullet weapon
            energyCounter.SetText("");
            heatCounter.SetText("");
            ammoCounter.SetText(bulletsLeft + "/" + magazineSize);
            magCounter.SetText(("" + magazinesLeft));
        }
        if (type == 2){ // energy weapon
            ammoCounter.SetText("");
            magCounter.SetText("");
            
            energyCounter.SetText("" + Mathf.RoundToInt(energy));
            heatCounter.SetText(Mathf.RoundToInt(heat) + "% / 100%");
        }
        if (type == 3){ // charging weapon
            ammoCounter.SetText("");
            magCounter.SetText("");

            energyCounter.SetText("" + Mathf.RoundToInt(energy));
            heatCounter.SetText(Mathf.RoundToInt(heat) + "% / 100%");
        }
    }

    public override void SetReloadingText(string text){
        base.SetReloadingText(text);
        ammoCounter.SetText(text);
    }

    private void VehicleEmbark(GameObject nearestObject){
        Vehicle vehicle = _allVehicles[nearestObject];
        if (IsTouching(transform.position, nearestObject.transform.position, vehicle.GetEmbarkRange(),
            vehicle.GetEmbarkRange())){
            pickupText.SetText("(G) " + nearestObject.name);
            if (Input.GetKeyDown(KeyCode.G)){
                SetNotifText("Embarked " + gameObject.name);
                gameObject.SetActive(false);

                vehicle.SetDriver(this);
                SpriteRenderer driver = vehicle.GetDriver();
                SpriteRenderer driverVisor = vehicle.GetDriverVisor();
                driver.enabled = true;
                driverVisor.enabled = true;

                Transform helmet = transform.GetChild(1).GetChild(5);
                driver.color = helmet.GetChild(0).GetComponent<SpriteRenderer>().color;
                driverVisor.color = helmet.GetChild(2).GetComponent<SpriteRenderer>().color;
                
                        
            }
        }
    }

    protected override void OnCollisionEnter2D(Collision2D other){
        base.OnCollisionEnter2D(other);
        
        if (other.gameObject.CompareTag("Ground")){
            if (hardLanding){
                SortSound(4);
            }
            else{
                SortSound(3);
            }
        }
    }
    
    
    public void AddWeapon(KeyValuePair<GameObject, KeyValuePair<BasicWeapon, Rigidbody2D>> weapon){
        _allBasicWeapons.Add(weapon.Key, weapon.Value);
    }
    public void AddVehicle(KeyValuePair<GameObject, Vehicle> vehicle){
        _allVehicles.Add(vehicle.Key, vehicle.Value);
    }
    
    public Dictionary<GameObject, KeyValuePair<BasicWeapon, Rigidbody2D>> _allBasicWeapons = new Dictionary<GameObject, KeyValuePair<BasicWeapon, Rigidbody2D>>();
    public Dictionary<GameObject, Vehicle> _allVehicles = new Dictionary<GameObject, Vehicle>();
    
    

    protected override void TakeDamage(int damage){
        base.TakeDamage(damage);
    }

    #endregion
    


    

    
}
