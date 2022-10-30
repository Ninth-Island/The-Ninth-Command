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
    
    [SerializeField] private int armorAbility;
    [SerializeField] private int maxCharge;
    [SerializeField] private int chargeRate;
    [SerializeField] private int chargeDrainPerFrame;
    
    [Header("Assault - Dash")]
    [SerializeField] private float dashVelocity;
    [SerializeField] private GameObject dashParticles;

    [Header("Heavy - Shield Overcharge")] 
    [SerializeField] private int overChargePerFrame;

    [Header("Operator - Team Shield")] 
    [SerializeField] private float distanceOfCharge;
    [SerializeField] private int teamChargePerFrame;
    [SerializeField] private GameObject fieldPrefab;


    [Header("Pathfinder - Cloak")] 
    [SerializeField] private float alpha;

    [Header("Sharpshooter - Jetpack")] 
    [SerializeField] private int jetPower;

    private bool _isArmorAbilitying; // for prolonged ones
    private bool _isModAbilitying;
    
    private bool _armorAbilityActive; // for one shots that have duration
    private bool _modAbilityActive;

    private int _currentAbilityCharge = 0;


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
            ArmorAbilityInstant();
        }

        if (Input.GetKey(KeyCode.LeftControl)){         
            _currentInput.AbilityInput = true;

        }
        
        if (Input.GetKeyUp(KeyCode.LeftControl)){         
            _currentInput.AbilityInput = false;

        }
        

        if (Input.GetKeyDown(KeyCode.Mouse5)){
            ModAbilityInstant();
        }
        
        if (Input.GetKey(KeyCode.Mouse5)){
            _currentInput.ModInput = true;
        }
        
        if (Input.GetKeyUp(KeyCode.Mouse5)){
            _currentInput.ModInput = false;
        }
        abilityChargeSlider.value = (float) _currentAbilityCharge / maxCharge;
    }


 
    private void AbilitiesFixedUpdate(){
        if (_armorAbilityActive){
            if (armorAbility == 1){
                OverchargeShield();
            }
            else if (armorAbility == 2){
                OverchargeTeam();
            }

            else if (armorAbility == 3){
                Cloak();
            }

            _currentAbilityCharge -= chargeDrainPerFrame;
            
            if (_currentAbilityCharge <= 0){
                _armorAbilityActive = false;
            }
        }
        else{
            _currentAbilityCharge = Mathf.Clamp(_currentAbilityCharge + chargeRate, 0, maxCharge);
        }
    }
    

    private void Dash(){

        float angle = arm.transform.rotation.eulerAngles.z;
        Destroy(Instantiate(dashParticles, transform.position + new Vector3(-0.37f, 2.05f), Quaternion.Euler(0, 0, angle)), 0.2f);
        angle *= Mathf.Deg2Rad;
        body.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dashVelocity;
    }

    private void OverchargeShield(){
        shield = Mathf.Clamp(shield += overChargePerFrame, 0, maxShield);
        if (isServer){
            UpdateHealthClientRpc(health, shield, true, false);
        }
    }

    private void OverchargeTeam(){
        foreach (TeammateHUDElements element in virtualPlayer.Team){
            Player p = element.VirtualPlayer.gamePlayer;
            if (Vector2.Distance(p.transform.position, transform.position) < distanceOfCharge){
                p.shield = Mathf.Clamp(p.shield += teamChargePerFrame, 0, p.maxShield);
                p.UpdateHealthClientRpc(p.health, p.shield, true, false);

            }
        }
    }

    private void Cloak(){
        Color color = bodyRenderer.color;
        color = new Color(color.r, color.g, color.b, 1 - ((float) _currentAbilityCharge / maxCharge));
        bodyRenderer.color = color;
    }

    private void Jetpack(){
        float angle = GetBarrelToMouseRotation();
        body.velocity += new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized * jetPower;
    }


    private void ArmorAbilityInstant(){
        if (_currentAbilityCharge >= maxCharge){
            if (armorAbility == 0){ // dash
                Dash();
                Debug.Log(_currentAbilityCharge);
                _currentAbilityCharge = 0;

            }
            else{
                _armorAbilityActive = true;
            }
        }
    }

    private void ArmorAbilityLong(){
        if (_isArmorAbilitying && chargeDrainPerFrame > 0){
            _currentAbilityCharge -= chargeDrainPerFrame;
            if (armorAbility == 4){ //jetpack
                Jetpack();
            }
        }
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
            ArmorAbilityLong();
            ModAbilityLong();
            AbilitiesFixedUpdate();
        }
    }

    private void ServerPlayerAbilitiesFixedUpdate(){
        ArmorAbilityLong();
        ModAbilityLong();
        AbilitiesFixedUpdate();}


}

