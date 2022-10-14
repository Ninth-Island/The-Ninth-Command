using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public partial class Player : Character{
    
    [Header("Basic Weapons")] 
    // primary weapon in character parent
    [SyncVar] public BasicWeapon secondaryWeapon;
    
    // primary weapon in character parent
    public BasicWeapon secondaryWeaponPrefab;
    
    
    private CursorControl _cursorControl; //used for cursor images and zooming
    
    private bool _swappedWeapon; // used to can't spam sounds

    private bool _attemptingToFire;
    private float _firingAngle;
    
    
    private float _lastArmAngle; // client only so barrel to mouse isn't constantly recalculated


    
    [Client]
    private void ClientPlayerWeaponUpdate(){
        if (Input.GetKey(KeyCode.R)){
            _currentInput.ReloadInput = true;
            primaryWeapon.Reload();
        }

        if (Input.GetKey(KeyCode.Mouse0)){
            _attemptingToFire = true;
            _firingAngle = _lastArmAngle * Mathf.Deg2Rad;

        }
        else if (Input.GetKeyUp(KeyCode.Mouse0)){
            _attemptingToFire = false;
            _firingAngle = 0;
        }
        
        ClientCheckSwap();
        CheckForPickup();
    }

    [Server]
    private void ServerPlayerWeaponFixedUpdate(){
        if (_lastInput.FiringInput){
            primaryWeapon.HandleFiring(_lastInput.FiringAngle);
            Debug.Log("server fire");
        }
    }

    [Client]
    private void ClientPlayerWeaponFixedUpdate(){
        if (_attemptingToFire){
            if (isClientOnly){
                primaryWeapon.HandleFiring(_firingAngle);
            }
            _currentInput.FiringAngle = _firingAngle;
            _currentInput.FiringInput = true;
        }
    }

    [Client]
    private void ClientWeaponControlStart(){
        _cursorControl = transform.GetChild(3).GetComponent<CursorControl>();
    }
    



    [ClientRpc]
    public void InitialWeaponOnClient(BasicWeapon pW){ // this is mostly for an edge case error
        primaryWeapon = pW;
    }
    
    

    [Command]
    private void CmdServerTellClientsSwap(){
        primaryWeapon.StopReloading();
        FinishReload();
        ClientReceiveSwap();
        HUDPickupWeapon();
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
    private void ClientCheckSwap(){
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

        if (Vector2.Distance(transform.position, nearestObject.transform.position) < 14){
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
    
    
    
    public float GetPlayerToMouseRotation(){
        if ((transform.position - _cursorControl.GetMousePosition()).magnitude < 3f){
            return 0;
        }
        float ang = Mathf.Atan2(_cursorControl.GetMousePosition().y - transform.position.y, _cursorControl.GetMousePosition().x - transform.position.x) * Mathf.Rad2Deg;
        if (ang < 0){
            return 360 + ang;
        }
        return ang;
    }

    private float GetBarrelToMouseRotation(){
        if ((transform.position - _cursorControl.GetMousePosition()).magnitude < 12f){
            return 0;
        }
        float ang = Mathf.Atan2(_cursorControl.GetMousePosition().y - primaryWeapon.firingPoint.position.y, _cursorControl.GetMousePosition().x - primaryWeapon.firingPoint.position.x) * Mathf.Rad2Deg;
        if (ang < 0){
            return 360 + ang;
        }
        return ang;
    }

}
