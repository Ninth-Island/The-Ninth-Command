using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MeleeWeapon : BasicWeapon{

    public int damage;
    [SerializeField] private float velocityMultiplier = 1f;
    [SerializeField] private int maxFrames;


    [SerializeField] private float energy;
    [SerializeField] private float energyPerSwing;
    [SerializeField] private MeleeHitBox hitBox;
    [SerializeField] private ParticleSystem top;
    [SerializeField] private ParticleSystem bottom;
    private int _framesTillReady;
    
    public override void HandleFiring(float angle){
        if (_framesTillReady <= 0 && energy > 0){
            HandleMagazineDecrement();
            wielder.body.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized * velocityMultiplier;
            if (isServer) ActivateHitboxClientRpc(true);
        }
    }

    public override void Ready(){
        audioManager.source.Play();
        base.Ready();
        
        gameObject.layer = wielder.gameObject.layer - 4;
        hitBox.gameObject.layer = wielder.gameObject.layer - 4;
        
        top.Play();
        bottom.Play();
    }

    public override void Drop(){
        base.Drop();
        hitBox.gameObject.layer = LayerMask.NameToLayer("Objects");
    }


    protected override void HandleMagazineDecrement(){
        base.HandleMagazineDecrement();
        energy -= energyPerSwing;
        _framesTillReady = maxFrames;
        StartCoroutine(ResetSwing());
    }

    private IEnumerator ResetSwing(){
        yield return new WaitForSeconds(1);
        if (isServer) ActivateHitboxClientRpc(false);
    }

    
    protected override void RefreshText(){
        wielder.SetWeaponValues(0, 0, 0, energy, Mathf.Clamp((float) _framesTillReady / maxFrames * 100, 0, 100), 2);
    }

    protected override void FixedUpdate(){
        _framesTillReady -= 1;
        base.FixedUpdate();
    }
    
    protected override void ClientUpdate(){
        base.ClientUpdate();
        if (activelyWielded){
            RefreshText();
        }
    }

    [ClientRpc]
    private void ActivateHitboxClientRpc(bool active){
        hitBox.gameObject.SetActive(active);
    }

    public override void PutAway(){
        base.PutAway();
        top.Stop();
        bottom.Stop();
        audioManager.source.Stop();
    }
}
