using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
    private float _speed;
    
    private AudioManager _audioManager;

    
    
    private SpriteRenderer _spriteRenderer;
    
    // Start is called before the first frame update
    void Start(){
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioManager = GetComponent<AudioManager>();
        if (_live){
            StartCoroutine(Fuse());
        }

        if (propulsion){
            Body.velocity *= 0.01f;
        }

    }

    private void FixedUpdate(){
        if (propulsion && _spriteRenderer.enabled){ // if propulsion rocket and active
            if (Body.velocity.magnitude <= 150){
                Body.velocity *= acceleration;
            }
        }
    }


    public void Explode(){
        _audioManager.PlayFromList(0);
        _spriteRenderer.enabled = false;
        Body.simulated = false;
        Instantiate(knockbackPrefab, transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(gameObject, 1f);
    }

    /*
     * ================================================================================================================
      *                                        For Sticky bombs, grenades and non-instant explosives
     * ================================================================================================================
     */
    IEnumerator Fuse(){
        Body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        gameObject.layer = LayerMask.NameToLayer("Ground");
        yield return new WaitForSeconds(fuseTimer);
        
        Explode();
    }
    
    public void SetFuseTimer(float setFuseTimer){
        fuseTimer = setFuseTimer;
    }

    public void StartFuse(){
        StartCoroutine(Fuse());
    }
    
/*
     * ================================================================================================================
      *                                        For impact explosives
     * ================================================================================================================
     */
    protected override void OnCollisionEnter2D(Collision2D other){
        base.OnCollisionEnter2D(other);
        if (impactGrenade){
            Explode();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
