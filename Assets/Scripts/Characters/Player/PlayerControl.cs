using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using TMPro;
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

    
    [Header("Heat and Energy")]
    [SerializeField] private float MaxEnergy;
    [SerializeField] private float energyCharge;
    [SerializeField] private float MaxHeat;
    [SerializeField] private float heatCharge;
    
    
    
    private float heat;
    private float energy;
    
    private Slider _energySlider;
    private Slider _overflowSlider;
    
    
    private AbilityNames _abilityNames = new AbilityNames();

    private Dictionary<KeyCode, Action> Bindings = new Dictionary<KeyCode, Action>();

    
    private Camera _mainCamera;
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
        
        _mainCamera = Camera.main;
        _cursorControl = FindObjectOfType<CursorControl>();


        Bindings.Add(KeyCode.LeftShift, Sprint);
        Bindings.Add(KeyCode.LeftControl, null);
        Bindings.Add(KeyCode.LeftAlt, Dash);
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

        pickupText.transform.position = Input.mousePosition;
    }


    private void ControlFixedUpdate(){
        heat = Mathf.Clamp(heat + heatCharge, 0, MaxHeat);
        energy = Mathf.Clamp(energy + energyCharge, 0, MaxEnergy);
        
        
        CheckBindings();
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
        if (Input.GetKey(KeyCode.W) && Airborne){
            Body.AddForce(Vector2.up * jetPower, ForceMode2D.Impulse);
        }
    }

    #endregion

    #region LeftControl

    private void Dash(){
        if (heat >= MaxHeat){
            heat = 0;
            float rotation = GetPlayerToMouseRotation();
            Vector2 dir = new Vector2(Mathf.Cos(rotation * Mathf.Deg2Rad), Mathf.Sin(rotation * Mathf.Deg2Rad)).normalized;
            Body.velocity = dir * dashVelocity;
        }
    }

    private void UseGrapplingHook(){
        
    }

    #endregion
   
    
  

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
    
    
    public BoxCollider2D GetFeetCollider(){
        return FeetCollider;
    }
    public float GetPlayerToMouseRotation(){
        float ang = Mathf.Atan2(_cursorControl.GetMousePosition().y - transform.position.y, _cursorControl.GetMousePosition().x - transform.position.x) * Mathf.Rad2Deg;
        if (ang < 0){
            return 360 + ang;
        }
        return ang;
    }
}

