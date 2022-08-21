using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Pathfinding.Ionic.Zip;
using Unity.Mathematics;
using UnityEngine;

public class Explosive : Projectile{
    
    /*
* ================================================================================================================
*                                               Explosive
*
*  Effective for dealing area of damage and destroying vehicles
     *
     * Contains logic for knockback, fuses for grenades and sticky bombs, and impact explosives
* 
* ================================================================================================================
*/

    [SerializeField] private float fuseTimer;
    [SerializeField] private Knockback knockbackPrefab;
    [SerializeField] private bool impactGrenade;
    [SerializeField] private bool propulsion = false;
    [SerializeField] private float acceleration;
    [SerializeField] private bool cat;
  
    
    private AudioManager _audioManager;

    
    
    private SpriteRenderer _spriteRenderer;
    
    // Start is called before the first frame update
    public override void OnStartClient(){
        base.OnStartClient();

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioManager = GetComponent<AudioManager>();
        

        if (propulsion){
            body.velocity *= 0.01f;
        }

    }

    public override IEnumerator SetValues(int damage, float speed, float angle, bool piercing, int firedLayer, string name){
        StartCoroutine(base.SetValues(damage, speed, angle, piercing, firedLayer, name));
        if (_live){
            StartCoroutine(Fuse());
        }
        yield break;
    }

    protected override void FixedUpdate(){
        if (propulsion && _spriteRenderer.enabled){ // if propulsion rocket and active
            
            if (body.velocity.magnitude <= 150){
                body.velocity *= acceleration;
            }
        }
    }


    public void Explode(){
       _audioManager.PlaySound(0, false);
        _spriteRenderer.enabled = false;
        body.simulated = false;
        Instantiate(knockbackPrefab, transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(gameObject, 1f);
    }

    /*
     * ================================================================================================================
      *                                        For Sticky bombs, grenades and non-instant explosives
     * ================================================================================================================
     */
    IEnumerator Fuse(){
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        gameObject.layer = _firedLayer - 4;
        yield return new WaitForSeconds(fuseTimer);
        
        Explode();
    }
    

    
/*
     * ================================================================================================================
      *                                        For impact explosives
     * ================================================================================================================
     */





    protected override void OnCollisionEnter2D(Collision2D other){
        if (cat){
            PhysicsMaterial2D material = new PhysicsMaterial2D();
            material.bounciness = body.sharedMaterial.bounciness * 1.1f;
            body.sharedMaterial = material;
            
            // don't uncomment this. Don't you dare.
            //_audioManager.PlaySound(0, false, 0);
        }
        else{
            base.OnCollisionEnter2D(other);
            if (impactGrenade){
                Explode();
            }
        }
    }

}
