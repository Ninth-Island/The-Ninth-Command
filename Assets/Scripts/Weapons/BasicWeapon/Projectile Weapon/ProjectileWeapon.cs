using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProjectileWeapon : BasicWeapon{
    
    /*
   * ================================================================================================================
   *                                  Projectile Weapon --> Basic Weapon --> Weapon
     *
     *   Contains logic for shooting and creating bullets
     * 
   * ================================================================================================================
   */
    
    
    [Header("Projectile Weapon")]
    [SerializeField] protected float firingDelay;
    [SerializeField] protected float salvoDelay;
    [SerializeField] protected float shotsPerSalvo;
    

    [SerializeField] protected float instability;

    [SerializeField] protected Projectile projectileTemplate;
    [SerializeField] protected int projectileDamage;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected bool piercing;




    
    protected bool ReadyToFire = true;
    protected bool Firing = false;
    
    
    
    
    protected IEnumerator Fire(float angle){
        Firing = true;
        if (ReadyToFire){
            for (int i = 0; i < shotsPerSalvo; i++){
                CreateProjectile(angle);
                SetLoadingState();
                yield return new WaitForSeconds(firingDelay);
            }
            ReadyToFire = false;
            StartCoroutine(SalvoDelay());
        }
    }


    private void CreateProjectile(float angle){
        Subtract();
        RefreshText();
        Projectile projectile = Instantiate(projectileTemplate, firingPoint.position, Quaternion.identity);
        Physics2D.IgnoreCollision(projectile.GetCollider(), wielder.GetCollider()); 
        Physics2D.IgnoreCollision(projectile.GetCollider(), wielder.GetFeetCollider());
        //projectile.GetComponent<Rigidbody2D>().velocity = wielder.GetBody().velocity;
        projectile.SetValues(projectileDamage, projectileSpeed, angle + Random.Range(-instability, instability), piercing, wielder.gameObject.layer, gameObject.name);
    }
    
    private IEnumerator SalvoDelay(){
        yield return new WaitForSeconds(salvoDelay);
        ReadyToFire = true;
        Firing = false;
    }

    

    protected override void Start(){
        base.Start();
        CursorControl = FindObjectOfType<CursorControl>();
    }

    private void Awake(){
        firingPoint = transform.GetChild(0);
    }

    protected override void Update(){
        base.Update();
    }
}
