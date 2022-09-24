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
    

    [Command]
    private void CmdSetFiring(bool firing, float angle){
        _attemptingToFire = firing;
        _firingAngle = angle;
    }


    #endregion

    #region Client


    /*
    * ================================================================================================================
    *                                               Start and update
    * ================================================================================================================
    */

    public override void OnStartClient(){
        
        HUDVisualStart();
        ControlStart();

        if (hasAuthority){
            _virtualCameras[0].Priority = 10;
            HUD.gameObject.SetActive(true);
        }
        base.OnStartClient();

    }

    
    protected override void ServerFixedUpdate(){
        base.ServerFixedUpdate();

        //ControlFixedUpdate();
        ServerFixedMove();
        if (_attemptingToFire){
            primaryWeapon.ServerHandleFiring(_firingAngle);
            
        }
        
    }

    protected override void ClientFixedUpdate(){
        base.ClientFixedUpdate();
        _lastArmAngle = GetBarrelToMouseRotation();
        ClientMoveFixedUpdate();
        
        if (Math.Abs(body.velocity.x) > 20 || Math.Abs(body.velocity.y) > 70){
            _hardLanding = true;
        }
        else{
            _hardLanding = false;
        }
    }
    
    protected override void ClientUpdate(){
        base.ClientUpdate();
        _lastArmAngle = GetBarrelToMouseRotation();
        ClientMoveUpdate();
        CheckSwap();
        
        SetAnimatedBoolOnAll(_aNames.jumping, Airborne);


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
            CmdSetFiring(true, _lastArmAngle * Mathf.Deg2Rad);
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
        UpdateHUD();
    }

    [ClientRpc]
    private void ClientReceiveSwap(){

        (primaryWeapon, secondaryWeapon) = (secondaryWeapon, primaryWeapon);

        primaryWeapon.spriteRenderer.enabled = true;

        secondaryWeapon.activelyWielded = false;
        secondaryWeapon.spriteRenderer.enabled = false;

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

    private void SetAnimatedBoolOnAll(string animationName, bool setTo){
        Animator.SetBool(animationName, setTo);
        CmdSetAnimatedBoolOnServer(animationName, setTo);
    }

    [Command]
    private void CmdSetAnimatedBoolOnServer(string animationName, bool setTo){
        SetAnimatedBoolOnClientRpc(animationName, setTo);
    }

    [ClientRpc]
    private void SetAnimatedBoolOnClientRpc(string animationName, bool setTo){
        Animator.SetBool(animationName, setTo);
    }
    #endregion
    


    

    
}
