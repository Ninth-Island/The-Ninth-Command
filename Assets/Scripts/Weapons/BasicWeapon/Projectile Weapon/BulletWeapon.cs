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
    




    [Server]
    protected override void ServerHandleFiring(float angle){
        if (bulletsLeft > 0){
            if (!reloading){
                base.ServerHandleFiring(angle);
            }
        }
        else{
            ServerReload();
        }
    }

    [Server]
    protected override void CreateProjectile(float angle){
        base.CreateProjectile(angle);
        if (bulletShell){
            StartCoroutine(CreateShell());
        }
    }

    [Server]
    private IEnumerator CreateShell(){
        GameObject gO = Instantiate(bulletShell, transform.position, transform.rotation);
        NetworkServer.Spawn(gO);
        yield return new WaitForSeconds(1);
        NetworkServer.Destroy(gO);
    }

    [Command]
    public override void CmdReload(){
        ServerReload();
    }

    [Server]
    private void ServerReload(){
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
    public IEnumerator ReloadRoutine(){
        reloading = true;
        base.CmdReload();
        
        wielder.Reload();
        wielder.SetReloadingText("Reloading...");


        yield return new WaitForSeconds(reloadTime);
        AudioManager.PlaySound(2, false);
        
        
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
    


    [Server]
    protected override void HandleMagazineDecrement(){
        base.HandleMagazineDecrement();
        bulletsLeft--;
    }
 
    
    

    [Client]
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
