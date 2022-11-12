using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BarrelShield : WeaponMod{

    [SerializeField] private ParticleSystem particles;
    protected override void OverrideInstant(){
        On();
        if (isServer){
            ServerRunOnClients(true);
        }
    }

    protected override void OutOfCharge(){
        Off();
        if (isServer){
            ServerRunOnClients(false);
        }
    }

    [Server]
    private void ServerRunOnClients(bool on){
        RunOnClientRpc(on);
    }

    [ClientRpc]
    private void RunOnClientRpc(bool on){
        if (!hasAuthority){
            if (on){
                On();
            }
            else{
                Off();
            }
        }
    }
    
    protected override void Start(){
        base.Start();
        Off();
    }

    private void On(){
        spriteRenderer.enabled = true;
        particles.Play();
        Collider.enabled = true;
        parent = WeaponAttachedTo.firingPoint;
    }

    private void Off(){
        spriteRenderer.enabled = false;
        particles.Stop();
        Collider.enabled = false;
    }
}

//https://www.youtube.com/watch?v=dnNCVcVS6uw&ab_channel=1MinuteUnity
//reloading isn't synced