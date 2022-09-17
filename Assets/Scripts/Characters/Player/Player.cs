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
    [SyncVar] public BasicWeapon secondaryWeapon;
    
    public BasicWeapon primaryWeaponPrefab;
    public BasicWeapon secondaryWeaponPrefab;

    [SyncVar] private bool _isCrouching;

    [SyncVar] private bool _hardLanding; // used for sound

    private bool _swappedWeapon; // used to can't spam sounds

    private bool _attemptingToFire;
    private float _firingAngle;
    
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
        _isCrouching = false;
    }

    [Command]
    private void CmdAnimatorUpdateAirborne(){ // only using authoritative animations for ease
        Animator.SetBool(_aNames.jumping, Airborne);
    }
    
    [Command]
    private void CmdToggleIsCrouching(){
        _isCrouching = !_isCrouching;
        Animator.SetBool(_aNames.crouching, _isCrouching);
    }

    [Command]
    private void CmdSetFiring(bool firing, float angle){
        _attemptingToFire = firing;
        _firingAngle = angle;
    }


    #endregion

    #region Client


    
    [Client]
    protected override void ClientHandleMove(){
        CmdSetXMoveServer(Input.GetAxis("Horizontal"));
    }

    [Client]
    protected override void ClientHandleJump(){
        if (Input.GetKeyDown(KeyCode.W)){
            CmdServerJump();
        }
    }

    [Client]
    private void CheckCrouch(){
        if (Input.GetKeyDown(KeyCode.S)){
            CmdToggleIsCrouching();
        }
    }

    /*
    * ================================================================================================================
    *                                               Start and update
    * ================================================================================================================
    */

    protected override void Start(){
        base.Start();

        HUDVisualStart();
        ControlStart();

        if (hasAuthority){
            _virtualCamera[0].Priority = 10;
            HUD.gameObject.SetActive(true);
        }
    }

    
    protected override void ServerFixedUpdate(){
        base.ServerFixedUpdate();

        //ControlFixedUpdate();

        if (_attemptingToFire){
            primaryWeapon.ServerHandleFiring(_firingAngle);
            
        }

        // just for sounds
        if (Math.Abs(body.velocity.x) > 20 || Math.Abs(body.velocity.y) > 70){
            _hardLanding = true;
        }
        else{
            _hardLanding = false;
        }
    }

    
    protected override void ClientUpdate(){
        base.ClientUpdate();

        CheckSwap();
        CheckCrouch();

        CmdAnimatorUpdateAirborne(); // has to happen on update in case walks off edge without jumping
        
        CmdRotateArm(GetBarrelToMouseRotation());

        ClientHandleWeapon();

        ControlUpdate(); // all hud and audio stuff
        HUDUpdate();
        CheckForPickup();
        
    }

    [Client]
    private void ClientHandleWeapon(){
        if (Input.GetKey(KeyCode.R)){
            primaryWeapon.CmdReload(); // make into a command at some point
        }

        if (Input.GetKey(KeyCode.Mouse0)){
            CmdSetFiring(true, GetBarrelToMouseRotation() * Mathf.Deg2Rad);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0)){
            CmdSetFiring(false, 0);
        }
    }

    [ClientRpc]
    public void InitialWeaponOnClient(BasicWeapon pW){ // this is mostly for an edge case error
        primaryWeapon = pW;
    }

    /*
    * ================================================================================================================
    *                                               Movement
    * ================================================================================================================
    */

    



    #endregion

    #region Weapon and Vehicle
    /*
    * ================================================================================================================
    *                                               Weapon and Vehicle
    * ================================================================================================================
    */
    

    [Command]
    private void CmdServerTellClientsSwap(){
        primaryWeapon.StopReloading();
        FinishReload();
        ClientReceiveSwap();
    }

    [ClientRpc]
    private void ClientReceiveSwap(){

        (primaryWeapon, secondaryWeapon) = (secondaryWeapon, primaryWeapon);

        primaryWeapon.spriteRenderer.enabled = true;

        secondaryWeapon.activelyWielded = false;
        secondaryWeapon.spriteRenderer.enabled = false;
        UpdateHUD();

        _swappedWeapon = true;
        Invoke(nameof(ResetSwappedWeapon), 0.25f);

    }
    
    [Client]
    private void CheckSwap(){

        if (Math.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0 && !Input.GetKey(KeyCode.Mouse1) && !_swappedWeapon){
            secondaryWeapon.CmdReady(); // IMPORTANT, there's a bit of delay between saying this and swapping weapons, so this is the soon to be primary
            CmdServerTellClientsSwap();
        }
    }

    [Client]
    private void ResetSwappedWeapon(){
        _swappedWeapon = false;
    }
    
    [Client]
    private void CheckForPickup(){
        Vector2 mousePos = _cursorControl.GetMousePosition();
        RaycastHit2D objectScan = Physics2D.CircleCast(mousePos, 1, new Vector2(), 0, LayerMask.GetMask("Objects", "Vehicle"));
        
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

    [Client]
    private void WeaponPickup(GameObject nearestObject){

        if (IsTouching(transform.position, nearestObject.transform.position, 10, 10)){
            pickupText.SetText("(G) " + nearestObject.name);
            if (Input.GetKeyDown(KeyCode.G)){
                BasicWeapon newWeapon = nearestObject.GetComponent<BasicWeapon>();
                
                newWeapon.CmdPickup(this, primaryWeapon, new []{1, 3});
                UpdateHUD();
            }
        }
    
    }
    

    [Client]
    public override void SetWeaponValues(int magazinesLeft, int magazineSize, int bulletsLeft, float energy, float heat, int type){ // HUD stuff
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

    [Client]
    public override void SetReloadingText(string text){
        base.SetReloadingText(text);
        ammoCounter.SetText(text);
    }

    [Client]
    private void VehicleEmbark(GameObject nearestObject){
        if (IsTouching(transform.position, nearestObject.transform.position, 50, 50)){
            pickupText.SetText("(G) " + nearestObject.name);
            if (Input.GetKeyDown(KeyCode.G)){
                SetNotifText("Embarked " + gameObject.name);
                gameObject.SetActive(false);
                Vehicle vehicle = nearestObject.GetComponent<Vehicle>();
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
            if (_hardLanding){
                SortSound(4);
            }
            else{
                SortSound(3);
            }
        }
    }

    public override void Hit(int damage){
        base.Hit(damage);
    }

    #endregion
    


    

    
}
