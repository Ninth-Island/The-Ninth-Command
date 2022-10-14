using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BulletWeapon : ProjectileWeapon{


    [Header("Bullet Weapon")]
    [SerializeField] private int magazinesLeft;
    [SerializeField] private int magazineSize;
    [SerializeField] private float reloadTime; // less frame dependent
    [SerializeField] private GameObject bulletShell;
    
    [SerializeField]    private int bulletsLeft;

    private bool reloading;
    





    public override void HandleFiring(float angle){
        if (bulletsLeft > 0){
            if (!reloading){
                base.HandleFiring(angle);
            }
        }
        else{
            Reload();
        }
    }


    protected override void CreateProjectile(float angle){
        base.CreateProjectile(angle);
    }



    public override void Reload(){
        if (magazinesLeft > 0 && bulletsLeft < magazineSize){
            if (!reloading){
                StartCoroutine(ReloadRoutine());
            }
        }
        else{
            AudioManager.PlayRepeating(3, 0); // dryfire
        }
    }

    [Server]
    public override void StopReloading(){
        base.StopReloading();
        reloading = false;
    }
    
    

    public IEnumerator ReloadRoutine(){
        reloading = true;
        wielder.SetReloadingText("Reloading...");
        
        wielder.Reload();


        yield return new WaitForSeconds(reloadTime);
        AudioManager.PlaySound(2, false);
        
        
        bulletsLeft = magazineSize;
        magazinesLeft--;
        wielder.FinishReload();
        reloading = false;

    }

    /*
     * ================================================================================================================
     *                                               Reloading
     * ================================================================================================================
     */
    


    protected override void HandleMagazineDecrement(){
        base.HandleMagazineDecrement();
        bulletsLeft--;
    }


    protected override void RefreshText(){
        if (reloading){
            wielder.SetReloadingText("Reloading...");
        }
        else{
            wielder.SetWeaponValues(magazinesLeft, magazineSize, bulletsLeft, 0, 0, 1);
        }
        
    }

    

    public override void OnStartClient(){
        base.OnStartClient();
    }


    protected override void FixedUpdate(){
        base.FixedUpdate();
    }
    
}
