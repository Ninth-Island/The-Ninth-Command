using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Repulse : WeaponMod{
    [SerializeField] private float velocity;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private PolygonCollider2D hitbox;
    
    protected override void OverrideInstant(){
        hitbox.enabled = true;
        particles.Play();
        StartCoroutine(ResetHitbox());
        Active = false;
        currentAbilityCharge = 0;    }


    private IEnumerator ResetHitbox(){
        yield return new WaitForSeconds(1);
        hitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D col){
        float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        col.attachedRigidbody.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized * velocity;
    }

    protected override void Start(){
        base.Start();
        particles.Stop();
    }
}
