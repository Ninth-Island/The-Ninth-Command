using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BulletWeapon : ProjectileWeapon{


    [Header("Bullet Weapon")]
    [SyncVar][SerializeField] private int magazinesLeft;
    [SyncVar][SerializeField] private int magazineSize;
    [SerializeField] private float reloadTime; // less frame dependent
    [SerializeField] private GameObject bulletShell;
    
    [SyncVar][SerializeField]    private int bulletsLeft;

    [SyncVar] private bool reloading;
    




    [Server]
    public override void ServerHandleFiring(float angle){
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
    }


    [Command]
    public override void CmdReload(){
        ServerReload();
    }

    [Server]
    public override void StopReloading(){
        base.StopReloading();
        reloading = false;
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
        SetReloadingText(connectionToClient, "Reloading...");
        
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
    


    [Server]
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


    [TargetRpc]
    private void SetReloadingText(NetworkConnection target, string text){
        wielder.SetReloadingText(text);
    }

    public override void OnStartClient(){
        base.OnStartClient();
    }


    protected override void FixedUpdate(){
        base.FixedUpdate();
    }
    
}
