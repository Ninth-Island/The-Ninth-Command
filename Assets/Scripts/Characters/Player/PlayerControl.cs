using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Pathfinding;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public partial class Player : Character{
    
    /*
* ================================================================================================================
*                                               PlayerControl
*
*  Parentally unrelated to player or character.
     *
     * Contains HUD (canvas) control for updating HUD text and getting mouse position
*
* 
* ================================================================================================================
*/
    [Header("Player Abilities")]
    [SerializeField] private float sprintAmplifier = 2;
    [SerializeField] private float jetPower;
    [SerializeField] private float dashVelocity;

    
    /*[Header("Heat and Energy")]
    [SerializeField] private float MaxEnergy;
    [SerializeField] private float energyCharge;
    [SerializeField] private float MaxHeat;
    [SerializeField] private float heatCharge;
    
    
    
    private float heat;
    private float energy;*/

    private GameObject _externalJetpack;

    private ParticleSystem _jetPackEffects;
    private TrailRenderer _foot1;
    private TrailRenderer _foot2;
    
    private Slider _energySlider;
    private Slider _overflowSlider;
    
    
    private AbilityNames _abilityNames = new AbilityNames();

    private Dictionary<KeyCode, Action> Bindings = new Dictionary<KeyCode, Action>();

    
    private Camera _mainCamera;
    private CinemachineVirtualCamera[] _virtualCamera;
    
    private CursorControl _cursorControl;
    private ANames _aNames = new ANames();



    #region Start and Update

        /*
    * ================================================================================================================
    *                                               Functions
    *
    *  Just separating because there's a lot of variables up there
    *
    * 
    * ================================================================================================================
    */
    
    protected virtual void ControlStart(){

        _externalJetpack = transform.GetChild(2).gameObject;
        _jetPackEffects = _externalJetpack.transform.GetChild(0).GetComponent<ParticleSystem>();
        _foot1 = _externalJetpack.transform.GetChild(1).GetComponent<TrailRenderer>(); 
        _foot2 = _externalJetpack.transform.GetChild(2).GetComponent<TrailRenderer>();        


        
        
        _mainCamera = Camera.main;
        _virtualCamera = new CinemachineVirtualCamera[3];
        _virtualCamera[0] = _mainCamera.transform.parent.GetChild(3).GetComponent<CinemachineVirtualCamera>();
        _virtualCamera[1] = _mainCamera.transform.parent.GetChild(4).GetComponent<CinemachineVirtualCamera>();
        _virtualCamera[2] = _mainCamera.transform.parent.GetChild(5).GetComponent<CinemachineVirtualCamera>();
        _cursorControl = FindObjectOfType<CursorControl>();


        Bindings.Add(KeyCode.LeftShift, UseJetPack);
        Bindings.Add(KeyCode.LeftAlt, Dash);
        Bindings.Add(KeyCode.Space, Fly);
    }
    
    protected virtual void ControlUpdate(){
        if (_fadeTimer > 0){
            _fadeTimer --;   
        }
        else{
            float alpha = _notificationText.color.a;
            if (alpha > 0){
                SetColor(0, 0, 0, _notificationText.color.a - fadeSpeed);
            }
        }
        
        
        if (Input.GetKeyUp(KeyCode.LeftShift)){
            if (AudioManager.source.clip == AudioManager.sounds[20].clipsList[0] ||
                AudioManager.source.clip == AudioManager.sounds[21].clipsList[0]){
                AudioManager.PlaySound(22, true, 0);
            }
        }

        pickupText.transform.position = Input.mousePosition;
    }


    private void ControlFixedUpdate(){/*
        heat = Mathf.Clamp(heat + heatCharge, 0, MaxHeat);
        energy = Mathf.Clamp(energy + energyCharge, 0, MaxEnergy);*/

        Body.constraints = RigidbodyConstraints2D.FreezeRotation;

        CheckBindings();

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, 0), 8);

        AudioSource source = AudioManager.source;

        if (!Input.GetKey(KeyCode.LeftShift)){
            _virtualCamera[0].Priority = 10;
            _virtualCamera[1].Priority = 0;
            _virtualCamera[2].Priority = 0;
        }

        if (Input.GetKey(KeyCode.LeftShift)){
            _jetPackEffects.Play();
        }
        else{
            _jetPackEffects.Stop();
        }

        _foot1.emitting = Input.GetKey(KeyCode.Space) && Airborne;
        _foot2.emitting = Input.GetKey(KeyCode.Space) && Airborne;


    }


    #endregion
    
    
    private void CheckBindings(){
        Animator.speed = 1f;

        foreach (KeyCode keyCode in Bindings.Keys){
            if (Input.GetKey(keyCode)){
                Bindings[keyCode].Invoke();
            }
        }
    }

    #region LeftShift

    
    private void Sprint(){
        Body.velocity = new Vector2(moveSpeed * sprintAmplifier * Input.GetAxis("Horizontal"), Body.velocity.y);
        Animator.speed = 1.3f;
    }
    
    
    private void UseJetPack(){
        Animator.SetBool(_aNames.jumping, true);
        if (AudioManager.source.clip == AudioManager.sounds[20].clipsList[0] || AudioManager.source.clip == AudioManager.sounds[21].clipsList[0]){
            if (AudioManager.source.time >= AudioManager.source.clip.length - 0.1f)
                AudioManager.PlaySound(21, true, 0);
        }
        else{
            AudioManager.PlaySound(20, true, 0);
        }

        if (!Input.GetKey(KeyCode.Space)){            
            
            
            _virtualCamera[0].Priority = 0;
            _virtualCamera[1].Priority = 10;
            _virtualCamera[2].Priority = 0;

            Body.AddForce(Vector2.up * jetPower, ForceMode2D.Impulse);
            if (Input.GetAxis("Horizontal") != 0){
                Sprint();
            }
        }
        else{            
            float rotation = GetPlayerToMouseRotation();
            rotation *= Mathf.Deg2Rad;
            Body.AddForce(new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)).normalized * jetPower, ForceMode2D.Impulse);
        }

    }

    private void Fly(){
        if (Airborne){
            
            float rotation = GetPlayerToMouseRotation() - 90;
            
            /*_virtualCamera[2].m_Lens.Dutch = transform.rotation.z * Mathf.Rad2Deg;
             */
         
            
            transform.rotation = Quaternion.Euler(0, 0, rotation );
            Body.constraints = RigidbodyConstraints2D.None;

            
            
            
            _virtualCamera[0].Priority = 0;
            _virtualCamera[1].Priority = 0;
            _virtualCamera[2].Priority = 10;
            
        }
    }

    

    #endregion

    #region LeftControl

    private void Dash(){
        //if (heat >= MaxHeat){
            //heat = 0;
            float rotation = GetPlayerToMouseRotation();
            Vector2 dir = new Vector2(Mathf.Cos(rotation * Mathf.Deg2Rad), Mathf.Sin(rotation * Mathf.Deg2Rad)).normalized;
            Body.velocity = dir * dashVelocity;
        //}
    }

    private void UseGrapplingHook(){
        
    }

    #endregion


    
    private void MagBoots(){
        //stick to walls and stuff. 
    }
    
    // similar to armor abilities, but this is the trigger for external equipment such as grav boots, jetpack, etc

  

    private class AbilityNames{
        public class LeftShift{
            public readonly string Sprint = "sprint";
            public readonly string Dash = "Dash";
        }

        public LeftShift leftShift = new LeftShift();

    }
    
    private class ANames{
        public readonly string running = "Running";
        public readonly  string runningBackwards = "RunningBackward";
        public readonly  string crouching = "Crouching";
        public readonly  string jumping = "Jumping";
        public readonly  string punching = "Punch";
        public readonly  string dying = "Dying";
    }
    

    public bool IsTouching(Vector2 pos1, Vector2 pos2, float xAffordance, float yAffordance){
        if (Math.Abs(pos1.x - pos2.x) < xAffordance && Math.Abs(pos1.y - pos2.y) < yAffordance){
            return true;
        }
        return false;
    }
    
    
    
    public float GetPlayerToMouseRotation(){
        float ang = Mathf.Atan2(_cursorControl.GetMousePosition().y - transform.position.y, _cursorControl.GetMousePosition().x - transform.position.x) * Mathf.Rad2Deg;
        if (ang < 0){
            return 360 + ang;
        }
        return ang;
    }
  
    public float GetBarrelToMouseRotation(){
        float ang = Mathf.Atan2(_cursorControl.GetMousePosition().y - primaryWeapon.firingPoint.position.y, _cursorControl.GetMousePosition().x - primaryWeapon.firingPoint.position.x) * Mathf.Rad2Deg;
        if (ang < 0){
            return 360 + ang;
        }
        return ang;
    }
}

