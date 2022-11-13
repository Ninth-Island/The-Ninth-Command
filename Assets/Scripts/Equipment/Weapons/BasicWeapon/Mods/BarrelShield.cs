using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BarrelShield : WeaponMod{

    [SerializeField] private ParticleSystem particles;
    protected override void OverrideInstant(){
        spriteRenderer.enabled = true;
        particles.Play();
        Collider.enabled = true;
        parent = WeaponAttachedTo.firingPoint;
        localPos = new Vector2(-1, 0);
    }

    protected override void OutOfCharge(){
        spriteRenderer.enabled = false;
        particles.Stop();
        Collider.enabled = false;;
    }

 
    
    protected override void Start(){
        base.Start();
        spriteRenderer.enabled = false;
        particles.Stop();
        Collider.enabled = false;;
    }

}

//https://www.youtube.com/watch?v=dnNCVcVS6uw&ab_channel=1MinuteUnity
//reloading isn't synced