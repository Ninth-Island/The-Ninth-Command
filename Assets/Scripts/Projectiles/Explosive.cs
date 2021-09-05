using System;
using System.Collections;
using System.Collections.Generic;
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
    
    
    private SpriteRenderer _spriteRenderer;
    
    // Start is called before the first frame update
    void Start(){
        _spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(Fuse());
    }
    
    
    public void Explode(){
        _spriteRenderer.enabled = false;
        Body.simulated = false;
        Instantiate(knockbackPrefab, transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(gameObject, 0.7f);
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
