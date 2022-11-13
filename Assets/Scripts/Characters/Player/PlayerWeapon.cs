using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public partial class Player : Character{
    
    [Header("Basic Weapons")] 
    // primary weapon in character parent
    public BasicWeapon secondaryWeapon;
    
    // primary weapon in character parent
    public BasicWeapon secondaryWeaponPrefab;
    
    
    private CursorControl _cursorControl; //used for cursor images and zooming
    
    private bool _swappedWeapon; // used to can't spam sounds

    private bool _attemptingToFire;
    private float _firingAngle;
    
    
    private float _lastArmAngle; // client only so barrel to mouse isn't constantly recalculated
    

    #region Firing

    
    [Server]
    private void ServerPlayerWeaponFixedUpdate(){
        if (_lastInput.FiringInput){
            primaryWeapon.HandleFiring(_lastInput.FiringAngle);
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

    #endregion
    

    #region swap

      
    [Client]
    private void ClientCheckSwap(){
        if (Math.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0 && !Input.GetKey(KeyCode.Mouse1) && !_swappedWeapon){
            _currentInput.SwapWeapon = true;
            
            HUDPickupWeapon(secondaryWeapon); // secondary weapon because this is before switching
            weaponImage.sprite = secondaryWeapon.spriteRenderer.sprite;
            if (isClientOnly){
                PlayerSwapWeapon();
            }

        }
    }

    [ClientRpc]
    private void PlayerSwapWeaponClientRpc(){ // for the server to update the clients after someone swaps
        if (!hasAuthority && !isServer){
            PlayerSwapWeapon();
        }
    }

    // shared
    private void PlayerSwapWeapon(){
        
        primaryWeapon.StopReloading();
        FinishReload();

        (primaryWeapon, secondaryWeapon) = (secondaryWeapon, primaryWeapon);

        primaryWeapon.spriteRenderer.enabled = true;

        secondaryWeapon.activelyWielded = false;
        secondaryWeapon.spriteRenderer.enabled = false;
        secondaryWeapon.PutAway();

        _swappedWeapon = true;
        Invoke(nameof(ResetSwappedWeapon), 0.25f);
        
        primaryWeapon.Ready();
        SetArmType(primaryWeapon.armType);

    }
    
    // shared
    private void ResetSwappedWeapon(){
        _swappedWeapon = false;
    }


    #endregion

    #region pickup

    [Client]
    private void CheckForPickup(){
        Vector2 mousePos = _cursorControl.GetMousePosition();
        RaycastHit2D objectScan = Physics2D.CircleCast(mousePos, 1, new Vector2(), 0, LayerMask.GetMask("Objects", "Vehicle"));
        
        pickupText.SetText("");
        if (objectScan){
            GameObject nearestObject = objectScan.collider.gameObject;
            pickupText.transform.position = _mainCamera.WorldToScreenPoint(nearestObject.transform.position);
            pickupText.SetText(nearestObject.name);

            if (nearestObject.CompareTag("Weapon")){
                ClientEquipmentPickup(nearestObject, 0);
            }
            if (nearestObject.CompareTag("Armor Ability")){
                ClientEquipmentPickup(nearestObject, 1);
            }

            if (nearestObject.CompareTag("Vehicle")){
                VehicleEmbark(nearestObject);
            }
        }
    }

    [Client]
    private void ClientEquipmentPickup(GameObject nearestObject, int equipmentType){
        if (Vector2.Distance(transform.position, nearestObject.transform.position) < 14){
            pickupText.SetText("(G) " + nearestObject.name);
            if (Input.GetKeyDown(KeyCode.G)){
                Equipment newEquipment;
                Equipment oldEquipment;
                int[] path ={0};
                if (equipmentType == 0){
                    BasicWeapon newWeapon = nearestObject.GetComponent<BasicWeapon>();
                    weaponImage.sprite = newWeapon.spriteRenderer.sprite;

                    oldEquipment = primaryWeapon;
                    primaryWeapon.StopReloading();
                    FinishReload();

                    newEquipment = newWeapon;
                    
                    SetArmType(newWeapon.armType);
                    HUDPickupWeapon(newWeapon);

                    path = new[]{0, 3};

                }
                else{
                    ArmorAbility newAbility = nearestObject.GetComponent<ArmorAbility>();
                    abilityImage.sprite = newAbility.spriteRenderer.sprite;

                    oldEquipment = armorAbility;
                    _currentInput.PickUpType = 1;

                    newEquipment = newAbility;

                }

                _currentInput.OldEquipment = oldEquipment;
                _currentInput.PickedUp = newEquipment;

                newEquipment.SwapTo(this, oldEquipment, path);
            }
        }
    }
    

    #endregion

    #region HUD stuff

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
        ammoCounter.SetText(text);
    }
    
    
    
    public float GetPlayerToMouseRotation(){
        float ang = Mathf.Atan2(_cursorControl.GetMousePosition().y - helmet.transform.position.y, _cursorControl.GetMousePosition().x - helmet.transform.position.x) * Mathf.Rad2Deg;
        if (ang < 0){
            return 360 + ang;
        }
        return ang;
    }

    public float GetBarrelToMouseRotation(){

        if ((transform.position - _cursorControl.GetMousePosition()).magnitude < 14 || _armOverrideReloading || _isSprinting){
            return GetPlayerToMouseRotation();
        }
        
        
        float ang = Mathf.Atan2(_cursorControl.GetMousePosition().y - primaryWeapon.firingPoint.position.y, _cursorControl.GetMousePosition().x - primaryWeapon.firingPoint.position.x) * Mathf.Rad2Deg;
        if (ang < 0){
            return 360 + ang;
        }
        return ang;
    }

    #endregion

    
    [Client]
    private void ClientPlayerWeaponUpdate(){
        if (Input.GetKey(KeyCode.R)){
            _currentInput.ReloadInput = true;
            primaryWeapon.Reload();
        }

        if (Input.GetKey(KeyCode.Mouse0) && !_isSprinting){
            _attemptingToFire = true;
            _firingAngle = _lastArmAngle * Mathf.Deg2Rad;

        }
        else if (Input.GetKeyUp(KeyCode.Mouse0)){
            _attemptingToFire = false;
            _firingAngle = 0;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && !_isSprinting){
            primaryWeapon.Zoom();
        }
        
        ClientCheckSwap();
        CheckForPickup();
    }


    [Client]
    private void ClientWeaponControlStart(){
        _cursorControl = transform.GetChild(1).GetComponent<CursorControl>();
    }
    

    [ClientRpc]
    public void InitializeEquipmentOnClient(BasicWeapon pW, BasicWeapon sW, ArmorAbility aa){ // this is mostly for an edge case error
        primaryWeapon = pW;
        secondaryWeapon = sW;
        armorAbility = aa;
        primaryWeapon.activelyWielded = true;
        aa.wielder = this;
        weaponImage.sprite = primaryWeapon.spriteRenderer.sprite;
    }
}

