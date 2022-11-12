using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using Pathfinding;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public partial class Player : Character{


    [Header("Abilities")] 
    
    public ArmorAbility armorAbility;
    public ArmorAbility defaultAbility;
    public WeaponMod startingModPrefab;
    [HideInInspector] public bool _isArmorAbilitying; // for prolonged ones
    [HideInInspector] private bool _isModAbilitying;
    




        /*
    * ================================================================================================================
    *                                               Functions
    *
    *  Just separating because there's a lot of variables up there
    *
    * 
    * ================================================================================================================
    */
        
    [Client]
    protected virtual void ClientPlayerAbilitiesUpdate(){
        
        if (Input.GetKeyDown(KeyCode.LeftControl)){
            float angle = GetBarrelToMouseRotation();
            if (isClientOnly) armorAbility.ArmorAbilityInstant(angle);
            
            _currentInput.AbilityInput = true;
            _currentInput.Angle = angle;
        }

        if (Input.GetKey(KeyCode.LeftControl)){         
            _currentInput.AbilityPressed = true;
        }
        
        if (Input.GetKeyUp(KeyCode.LeftControl)){         
            armorAbility.ArmorAbilityReleased(); // this can't be client only cuz there isn't dedicated server code for it
            _currentInput.AbilityPressed = false;
        }


        if (Input.GetKeyDown(KeyCode.Mouse4)){
            if (isClientOnly) primaryWeapon.weaponMod.WeaponModInstant();
            _currentInput.ModInput = true;
        }

        if (Input.GetKey(KeyCode.Mouse4)){
            _currentInput.ModPressed = true;
        }
        
        if (Input.GetKeyUp(KeyCode.Mouse4)){
            primaryWeapon.weaponMod.WeaponModRelease();
            _currentInput.ModPressed = false;
        }
        abilityChargeSlider.value = (float) armorAbility.currentAbilityCharge / armorAbility.maxCharge;
    }
    
    

    private void ClientPlayerAbilitiesFixedUpdate(){
        if (isClientOnly){
            armorAbility.ArmorAbilityFixedUpdate();
            primaryWeapon.weaponMod.WeaponModFixedUpdate();
        }
    }

    private void ServerPlayerAbilitiesFixedUpdate(){
        armorAbility.ArmorAbilityFixedUpdate();
        primaryWeapon.weaponMod.WeaponModFixedUpdate();
    }


}

