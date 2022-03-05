using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : BasicWeapon{

    [SerializeField] private int damage;
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float attackLength = 1f;
    [SerializeField] private float coolDown;

    [SerializeField] private bool hasActiveSound;
    [SerializeField] private Vector2 leftRightRotation;
    [SerializeField] private Vector2 leftRightRotationSwing;

    private bool _isAttacking;
    private float _coolDownLeft = 0;

    private MeleeHitBox _hitBox;

    
    protected override void Start(){
        base.Start();
        _hitBox = transform.GetChild(0).GetComponent<MeleeHitBox>();
    }

    
    
    
    
    
    
    /*
     *
     *
     *yeah so there isn't actually any hit detection on here yet. I'm thinking probably create a child that has all the knockback properties and stuff
     *
     * 
     */
    
    protected override void FixedUpdate(){
        base.FixedUpdate();
        if (_coolDownLeft > 0){
            _coolDownLeft -= 0.1f;
            
        }
    }

    protected override void Update(){
        base.Update();
        if (Player.primaryWeapon == this){
            _hitBox.SetLayer(Player.gameObject.layer - 4);
            if (!_isAttacking){
                Player.SetArmRotation(leftRightRotation);
            }
            else{
                SwingTo();
            }

            if (hasActiveSound && !AudioManager.source.isPlaying){
                AudioManager.PlaySound(1, false, 0);
            }
        }
    }

    protected override void CheckFire(){
        if (!_isAttacking && _coolDownLeft <= 0){
            StartCoroutine(Swish());
        }
    }

    private IEnumerator Swish(){
        AudioManager.PlaySound(0, false, 0);
        
        _hitBox.gameObject.SetActive(true);
        Player.moveSpeed *= speedMultiplier;
        _isAttacking = true;
        
        yield return new WaitForSeconds(attackLength);

        _hitBox.gameObject.SetActive(false);
        Player.moveSpeed /= speedMultiplier;
        _isAttacking = false;
        
        _coolDownLeft = coolDown;
    }


    private void SwingTo(){
        Player.SetArmRotation(leftRightRotationSwing);
        
    }

    
}
