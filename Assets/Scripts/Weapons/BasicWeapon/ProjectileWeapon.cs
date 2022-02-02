﻿using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;

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

    [SerializeField] private int zoomLevel;
    [SerializeField] private float zoomRange;
    [SerializeField] private float zoomLength;
    [SerializeField] private float zoomArc;

    private CursorControl _cursorControl;
    
    public Transform firingPoint;

    
    protected bool ReadyToFire = true;
    protected bool Firing = false;
    
    
    
    
    protected IEnumerator Fire(){
        Firing = true;
        if (ReadyToFire){
            for (int i = 0; i < shotsPerSalvo; i++){
                CreateProjectile();
                SetLoadingState();
                yield return new WaitForSeconds(firingDelay);
            }
            ReadyToFire = false;
            StartCoroutine(SalvoDelay());
        }
    }


    private void CreateProjectile(){
        Subtract();
        RefreshText();
        Projectile projectile = Instantiate(projectileTemplate, firingPoint.position, Quaternion.identity);
        Physics2D.IgnoreCollision(projectile.GetCollider(), Player.GetCollider()); 
        Physics2D.IgnoreCollision(projectile.GetCollider(), Player.GetFeetCollider());
        projectile.GetComponent<Rigidbody2D>().velocity = Player.GetBody().velocity;
        projectile.SetValues(projectileDamage, projectileSpeed, Player.GetPlayerToMouseRotation() * Mathf.Deg2Rad + Random.Range(-instability, instability), piercing, wielder.gameObject.layer, gameObject.name);
    }
    
    private IEnumerator SalvoDelay(){
        yield return new WaitForSeconds(salvoDelay);
        ReadyToFire = true;
        Firing = false;
    }

    private void CheckZoom(){
        _cursorControl.ResetCamera();
        if (Input.GetKey(KeyCode.Mouse1)){
            if (zoomLevel > 0){
                _cursorControl.CameraFollow(zoomLevel, zoomRange);
            }
        }
    }

    protected override void Start(){
        base.Start();
        firingPoint = transform.GetChild(0);
        
        _cursorControl = FindObjectOfType<CursorControl>();

    }

    protected override void Update(){
        if (Player.primaryWeapon == this){
            base.Update();
            CheckZoom();
        }
    }
}
