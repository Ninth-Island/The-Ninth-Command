using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletWeapon : ProjectileWeapon{


    [Header("Bullet Weapon")]
    [SerializeField] private int magazinesLeft;
    [SerializeField] private int magazineSize;
    [SerializeField] private float reloadTime; // less frame dependent
    [SerializeField] private GameObject bulletShell;
    
    [SerializeField]    private int bulletsLeft;

    private bool reloading;
    




    public override void AttemptFire(float angle){
        if (bulletsLeft > 0){
            if (!reloading){
                base.AttemptFire(angle);
            }
        }
        else{
            Reload();
        }
    }

    protected override void CreateProjectile(float angle){
        base.CreateProjectile(angle);
        if (bulletShell){
            Destroy(Instantiate(bulletShell, transform.position, transform.rotation), 1f);
        }
    }

    public override void Reload(){
        if (magazinesLeft > 0 && bulletsLeft < magazineSize){
            if (!reloading){
                StartCoroutine(ReloadRoutine());
            }
        }
        else{
            audioManager.PlayRepeating(3, 0); // dryfire
        }
    }
    
    public IEnumerator ReloadRoutine(){
        reloading = true;
        base.Reload();
        
        wielder.Reload();
        wielder.SetReloadingText("Reloading...");


        yield return new WaitForSeconds(reloadTime);
        audioManager.PlaySound(2, false);
        
        
        bulletsLeft = magazineSize;
        magazinesLeft--;
        RefreshText();
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
 
    
    

    public override void RefreshText(){
        if (wielder){
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
