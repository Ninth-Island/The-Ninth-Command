using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveMod : WeaponMod{
    
    protected bool IsReady;
    private Player _player;
    
    protected override void Start(){
        base.Start();
        _player = FindObjectOfType<Player>();
    }

    protected override void Update(){
        base.Update();
        //IsReady = _player.primaryWeapon == WeaponAttachedTo;
    }
}


/*Ideas:
 
    Grappling Hook
    Mounted Energy Shield
    Repulse
    Weapon Lock
    
    
    extra fire:
        micro missiles*/
