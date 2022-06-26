using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] public BasicWeapon primaryWeapon;
    [SerializeField] public BasicWeapon secondaryWeapon;
    
    
    private bool _isCrouching;

    private bool hardLanding;


    #region Start And Update

    /*
    * ================================================================================================================
    *                                               Start and update
    * ================================================================================================================
    */

    protected override void Start(){
        base.Start();

        HUDVisualStart();
        ControlStart();
    }

    protected override void FixedUpdate(){
        base.FixedUpdate();
        Move();
        Jump();
        
        if (Input.GetKey(KeyCode.Mouse0)){
            primaryWeapon.CheckFire(GetBarrelToMouseRotation() * Mathf.Deg2Rad);
        }
        
        ControlFixedUpdate();

        hardLanding = false;
        if (Math.Abs(Body.velocity.x) > 20 || Math.Abs(Body.velocity.y) > 70){
            hardLanding = true;
        }
        
    }

    protected override void Update(){
        base.Update();

        CheckSwap();
        
        CheckCrouch();
        primaryWeapon.CheckReload();
        RotateArm();
        
        ControlUpdate();
        CheckForPickup();
    }
    #endregion

    #region Movement
    /*
    * ================================================================================================================
    *                                               Movement
    * ================================================================================================================
    */

    private void Move(){
        float input = Input.GetAxis("Horizontal");
        Animator.SetBool(_aNames.running, input != 0);

        if (input != 0 && !InputsFrozen && !FallingKnocked){

            Animator.SetBool(_aNames.runningBackwards, Math.Sign(input) != Math.Sign(transform.localScale.x));
            
            if (_isCrouching){
                Body.velocity = new Vector2(moveSpeed / 2 * input, Body.velocity.y);
                SortSound(2);
            }
            else{
                Body.velocity = new Vector2(moveSpeed * input, Body.velocity.y);
            }
            
        }
    }

    private void Jump(){
        
        if (Input.GetKey(KeyCode.W)){
            Vector2 velocity = Body.velocity;
            
            // can't use airborne because the player is considered not airborne a few seconds before and after jumping
            if (FeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground", "Platform", "Vehicle", "Vehicle Outer"))){
                Airborne = true;
                Body.velocity = new Vector2(velocity.x, velocity.y + jumpPower);
                
                SortSound(0);
            }
        }
        Animator.SetBool(_aNames.jumping, Airborne);
    }


    private void CheckCrouch(){
        if (Input.GetKeyDown(KeyCode.S)){
            _isCrouching = !_isCrouching;
            Animator.SetBool(_aNames.crouching, _isCrouching);
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

        if (Math.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0 && !Input.GetKey(KeyCode.Mouse1)){
            if (primaryWeapon != null){
                primaryWeapon.SetSpriteRenderer(false);
            }

            if (secondaryWeapon != null){
                secondaryWeapon.SetSpriteRenderer(true);
            }

            primaryWeapon.SetLoadingState();

            (primaryWeapon, secondaryWeapon) = (secondaryWeapon, primaryWeapon);

            primaryWeapon.PickUp(this);
            UpdateHUD();
        }
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
                weapon.PickUp(this);
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
