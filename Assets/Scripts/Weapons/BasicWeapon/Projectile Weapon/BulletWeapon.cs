﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletWeapon : ProjectileWeapon{
    
    /*
   * ================================================================================================================
   *                                  Bullet Weapon --> Basic Weapon --> Weapon
     *
     *  Bullets within a magazine, bullets, reloading
     * 
   * ================================================================================================================
   */
    
    
    [Header("Bullet Weapon")]
    [SerializeField] private int magazinesLeft;
    [SerializeField] private int magazineSize;
    [SerializeField] private float reloadTime;
    [SerializeField] private GameObject bulletShell;
    
    public bool reloading;
    private int _bulletsLeft;
    
    
    /*
     * ================================================================================================================
     *                                               Firing Stuff
     * ================================================================================================================
     */
    
    protected override void Start(){
        base.Start();
        _bulletsLeft = magazineSize;
    }


    protected override void FixedUpdate(){
        base.FixedUpdate();
    }


    protected override void CheckFire(){
        if (_bulletsLeft > 0 && !reloading && !Firing){
            StartCoroutine(Fire());
            if (bulletShell){
                Destroy(Instantiate(bulletShell, transform.position, transform.rotation), 1f);
            } 
        }
        else if (!reloading && _bulletsLeft <= 0){
            if (magazinesLeft > 0){
                StartReloading();
            }
            else{
                AudioManager.PlaySound(3, true, 0);
            }
        }
    }

    
    public IEnumerator Reload(){
        if (magazinesLeft > 0){
            
            
            Player.ammoCounter.SetText("Reloading...");
            
            
            reloading = true;
            AudioManager.PlaySound(1, false, 0);
            yield return new WaitForSeconds(reloadTime);
            AudioManager.PlaySound(2, false, 0);
            
            
            _bulletsLeft = magazineSize;
            reloading = false;
            magazinesLeft--;
            RefreshText();
        }
    }

    /*
     * ================================================================================================================
     *                                               Reloading
     * ================================================================================================================
     */
    
    
    
    public void StartReloading(){
        Coroutine = StartCoroutine(Reload());
    }
    
    public void StopReloading(){
        if (reloading){
            StopCoroutine(Coroutine);
        }
    }

    public override void CheckReload(){
        base.CheckReload();
        if (Input.GetKeyDown(KeyCode.R) && magazineSize != _bulletsLeft && !reloading){
            StartReloading();
        }
    }

    public override void SetLoadingState(){
        base.SetLoadingState();
        StopReloading();
        reloading = false;
    }

    protected override void Subtract(){
        base.Subtract();
        _bulletsLeft--;
    }
    
    /*
    * ================================================================================================================
    *                                               Other
    * ================================================================================================================
    */

    public override void RefreshText(){
        if (Player.primaryWeapon == this){
            base.RefreshText();
            Player.energyCounter.SetText("");
            Player.heatCounter.SetText("");
            Player.ammoCounter.SetText(_bulletsLeft + "/" + magazineSize);
            Player.magCounter.SetText(("" + magazinesLeft));
        }
    }

    /*
   * ================================================================================================================
   *                                               Getters and Setters
   * ================================================================================================================
   */

    public int GetBulletsLeft(){
        return _bulletsLeft;
    }
    public int GetMagazineSize(){
        return magazineSize;
    }
    public int GetMagazinesLeft(){
        return magazinesLeft;
    }

    
}