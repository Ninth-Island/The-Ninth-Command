using System.Collections;
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
    
    [SerializeField]    private int bulletsLeft;
    public bool reloading;
    
    
    /*
     * ================================================================================================================
     *                                               Firing Stuff
     * ================================================================================================================
     */
    
    protected override void Start(){
        base.Start();
    }


    protected override void FixedUpdate(){
        base.FixedUpdate();
    }


    public override void CheckFire(float angle){
        if (bulletsLeft > 0 && !reloading && !Firing){
            StartCoroutine(Fire(angle));
            if (bulletShell){
                Destroy(Instantiate(bulletShell, transform.position, transform.rotation), 1f);
            } 
        }
        else if (!reloading && bulletsLeft <= 0){
            if (magazinesLeft > 0){
                StartReloading();
            }
            else{
                AudioManager.PlayRepeating(3, 0);
            }
        }
    }

    
    public IEnumerator Reload(){
        wielder.Reload();
        if (magazinesLeft > 0){
            wielder.SetReloadingText("Reloading...");
            base.CheckReload();


            reloading = true;
            yield return new WaitForSeconds(reloadTime);
            AudioManager.PlaySound(2, false, 0);
            
            
            bulletsLeft = magazineSize;
            reloading = false;
            magazinesLeft--;
            RefreshText();
            wielder.FinishReload();
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
        if (Input.GetKeyDown(KeyCode.R) && magazineSize != bulletsLeft && !reloading){
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
        bulletsLeft--;
    }
    
    /*
    * ================================================================================================================
    *                                               Other
    * ================================================================================================================
    */

    public override void RefreshText(){
        if (wielder){
            wielder.SetWeaponValues(magazinesLeft, magazineSize, bulletsLeft, 0, 0, 1);
        }
    }

    /*
   * ================================================================================================================
   *                                               Getters and Setters
   * ================================================================================================================
   */

    public int GetBulletsLeft(){
        return bulletsLeft;
    }
    public int GetMagazineSize(){
        return magazineSize;
    }
    public int GetMagazinesLeft(){
        return magazinesLeft;
    }

    
}
