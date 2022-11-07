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
            if (isClientOnly) armorAbility.ArmorAbilityReleased();
            _currentInput.AbilityPressed = false;
        }


        if (Input.GetKeyDown(KeyCode.Mouse5)){
            if (isClientOnly) ModAbilityInstant();
            _currentInput.ModInput = true;
        }

        if (Input.GetKey(KeyCode.Mouse5)){
            _currentInput.ModPressed = true;
        }
        
        if (Input.GetKeyUp(KeyCode.Mouse5)){
            _currentInput.ModPressed = false;
        }
        abilityChargeSlider.value = (float) armorAbility.currentAbilityCharge / armorAbility.maxCharge;
    }
    
    
    
    private void ModAbilityInstant(){
        Debug.Log("instant mod ability");
    }

    private void ModAbilityLong(){
        if (_isModAbilitying){
            Debug.Log("continous mod");
        }
    }

    private void ClientPlayerAbilitiesFixedUpdate(){
        if (isClientOnly){
            armorAbility.ArmorAbilityFixedUpdate();
            ModAbilityLong();
        }
    }

    private void ServerPlayerAbilitiesFixedUpdate(){
        armorAbility.ArmorAbilityFixedUpdate();
        ModAbilityLong();
    }


}

