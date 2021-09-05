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


    protected override void CheckFire(){
        if (_bulletsLeft > 0 && !reloading && !Firing){
            StartCoroutine(Fire());
        }
        else if (!reloading && _bulletsLeft <= 0){
            StartReloading();
        }
    }

    
    public IEnumerator Reload(){
        if (magazinesLeft > 0){
            PlayerPickupController.ammoCounter.SetText("Reloading...");
            reloading = true;
            yield return new WaitForSeconds(reloadTime);
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
        if (Input.GetKeyDown(KeyCode.R) && magazineSize != _bulletsLeft && !reloading){
            StartReloading();
        }
    }

    public override void SetLoadingState(){
        StopReloading();
        reloading = false;
    }

    protected override void Subtract(){
        _bulletsLeft--;
    }
    
    /*
    * ================================================================================================================
    *                                               Other
    * ================================================================================================================
    */
    
    public override void RefreshText(){
        base.RefreshText();
        PlayerPickupController.energyCounter.SetText("");
        PlayerPickupController.heatCounter.SetText("");
        PlayerPickupController.ammoCounter.SetText(_bulletsLeft + "/" + magazineSize);
        PlayerPickupController.magCounter.SetText(("" + magazinesLeft));
    }
    // Update is called once per frame
    protected override void Update(){
        base.Update();
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
