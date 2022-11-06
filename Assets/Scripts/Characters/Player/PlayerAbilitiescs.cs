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
    [SerializeField] private float fieldScale;
    [SerializeField] private Vector2 fieldOffset;

    [Header("Pathfinder - Cloak")] 
    [SerializeField] private float cloakedMoveSpeed;

    [Header("Sharpshooter - Jetpack")] 
    [SerializeField] private int jetPower;
    [SerializeField] private float maximumRise;
    [SerializeField] private ParticleSystem jetpack;

    private bool _isArmorAbilitying; // for prolonged ones
    private bool _isModAbilitying;
    
    private bool _armorAbilityActive; // for one shots that have duration
    private bool _modAbilityActive;

    private int _currentAbilityCharge = 0;

    private int _jetpackPhase;


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
            if (isClientOnly){
                ArmorAbilityInstant(angle);
            }
            _currentInput.AbilityInput = true;
            _currentInput.Angle = angle;
        }

        if (Input.GetKey(KeyCode.LeftControl)){         
            _currentInput.AbilityPressed = true;
        }
        
        if (Input.GetKeyUp(KeyCode.LeftControl)){         
            _currentInput.AbilityPressed = false;
            if (_jetpackPhase != 0){
                AudioManager.source.Stop();
                AudioManager.PlaySound(29);
                _jetpackPhase = 0;
                jetpack.Stop();
            }
        }


        if (isClientOnly && Input.GetKeyDown(KeyCode.Mouse5)){
            if (isClientOnly){
                ModAbilityInstant();
                _currentInput.ModInput = true;
            }
        }

        if (Input.GetKey(KeyCode.Mouse5)){
            _currentInput.ModPressed = true;
        }
        
        if (Input.GetKeyUp(KeyCode.Mouse5)){
            _currentInput.ModPressed = false;
        }
        abilityChargeSlider.value = (float) _currentAbilityCharge / maxCharge;
    }


 
    private void AbilitiesFixedUpdate(){
        if (armorAbility != 4){
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
        else{
            _currentAbilityCharge = Mathf.Clamp(_currentAbilityCharge + chargeRate, 0, maxCharge);
        }
    }
    

    private void Dash(float angle){
        Destroy(Instantiate(dashParticles, transform.position + new Vector3(-0.37f, 2.05f), Quaternion.Euler(0, 0, angle)), 0.2f);
        angle *= Mathf.Deg2Rad;
        body.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized * dashVelocity;
        FallingKnocked = true;
        
        AudioManager.PlaySound(27);
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
                if (isServer){
                    p.UpdateHealthClientRpc(p.health, p.shield, true, false);
                }
            }
        }
    }

    private void Cloak(){
        float a = 1 - ((float) _currentAbilityCharge * 3 / maxCharge);
        
        SetCamoColor(bodyRenderer, a);
        SetCamoColor(armRenderer, a);
        SetCamoColor(helmetRenderer, a);
        SetCamoColor(visorRenderer, a);
        SetCamoColor(primaryWeapon.spriteRenderer, a);
    }

    private void SetCamoColor(SpriteRenderer sR, float a){
        Color color = sR.color;
        color = new Color(color.r, color.g, color.b, a);
        sR.color = color;
    }

    private void Jetpack(){
        if (_jetpackPhase == 0){
            AudioManager.PlayLooping(27);
            StartCoroutine(SetNextJetpackNoise());
            jetpack.Play();
        }

        if (_jetpackPhase == 2){
            AudioManager.PlayLooping(28);
        }
        body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y + jetPower, -maximumRise, maximumRise));
    }

    private IEnumerator SetNextJetpackNoise(){
        _jetpackPhase = 1;
        yield return new WaitForSeconds(4.061f);
        _jetpackPhase = 2;
    }

    [ClientRpc]
    private void CreateFieldClientRpc(Transform p, Vector2 offset, float scale, float time){
        GameObject field = Instantiate(fieldPrefab, p);
        field.transform.localPosition = offset;
        field.transform.localScale = new Vector3(scale, scale);
        Destroy(field, time);
        
        AudioManager.PlayNewSource(27, time);
    }

    private void ArmorAbilityInstant(float angle){
        if (_currentAbilityCharge >= maxCharge){
            if (armorAbility == 0){ // dash
                Dash(angle);
                _currentAbilityCharge = 0;

            }
            else{
                _armorAbilityActive = true;
                if (armorAbility == 1 || armorAbility == 2){
                    if (isServer){
                        CreateFieldClientRpc(transform, fieldOffset, fieldScale, (float) maxCharge / chargeDrainPerFrame / 50);
                    }
                }
                else if (armorAbility == 3){
                    StartCoroutine(ResetCloak());
                }
            }
        }
    }

    private IEnumerator ResetCloak(){
        float tempSpeed = moveSpeed;
        moveSpeed = cloakedMoveSpeed;
        floatingCanvas.SetActive(false);
        
        AudioManager.PlaySound(27);
        
        yield return new WaitForSeconds((float) maxCharge / chargeDrainPerFrame / 50);
        SetCamoColor(bodyRenderer, 1);
        SetCamoColor(armRenderer, 1);
        SetCamoColor(helmetRenderer, 1);
        SetCamoColor(visorRenderer, 1);
        SetCamoColor(primaryWeapon.spriteRenderer, 1);
        moveSpeed = tempSpeed;

        floatingCanvas.SetActive(true);
    }

    private void ArmorAbilityLong(){
        if (_isArmorAbilitying && _currentAbilityCharge > 0){
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

