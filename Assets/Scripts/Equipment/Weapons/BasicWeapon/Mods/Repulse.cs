using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repulse : WeaponMod{
    [SerializeField] private float velocity;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private PolygonCollider2D hitbox;
    
    public override void WeaponModInstant(){
        hitbox.enabled = true;
        particles.Play();
        StartCoroutine(ResetHitbox());
    }

    private IEnumerator ResetHitbox(){
        yield return new WaitForSeconds(1);
        hitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D col){
        float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        col.attachedRigidbody.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized * velocity;
        Debug.Log(col.attachedRigidbody.velocity);
    }

    protected override void Start(){
        base.Start();
        particles.Stop();
    }
}
