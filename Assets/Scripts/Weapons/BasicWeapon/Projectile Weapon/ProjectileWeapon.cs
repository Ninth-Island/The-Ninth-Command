using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProjectileWeapon : BasicWeapon{


    [Header("Projectile Weapon")]
    [SerializeField] protected int shotDelay;
    [SerializeField] protected int salvoDelay;
    [SerializeField] protected int shotsPerSalvo;
    

    [SerializeField] protected float instability;

    [SerializeField] protected Projectile projectileTemplate;
    [SerializeField] protected int projectileDamage;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected bool piercing;


    private int framesLeftTillNextShot; // has to be counted in physics update for consistent firing
    private int framesLeftTillNextSalvo;

    private int shotInSolvo;

    private float shootingAngle;

    
    
    public override void HandleFiring(float angle){
        shootingAngle = angle;
        if (framesLeftTillNextSalvo <= 0){

            if (framesLeftTillNextShot <= 0){
                
                CreateProjectile(angle, true, GetSeed());
                HandleMagazineDecrement();

                framesLeftTillNextShot = shotDelay;
                shotInSolvo++;
                if (shotInSolvo >= shotsPerSalvo){
                    framesLeftTillNextSalvo = salvoDelay;
                    shotInSolvo = 0;
                }
            }
        }        
    }
    
        
    
    private void CreateProjectile(float angle, bool original, int seed){

        Projectile projectile = Instantiate(projectileTemplate, firingPoint.position, Quaternion.identity);
        
        Random.InitState(seed);
        projectile.SetValues(wielder, projectileDamage, projectileSpeed, angle + Random.Range(-1f, 1f) * instability, piercing, wielder.gameObject.layer, gameObject.name);
    
        if (isServer && original){
            SpawnProjectileOnOtherClientsRpc(angle, seed);
        }
    }


    [ClientRpc]
    private void SpawnProjectileOnOtherClientsRpc(float angle, int seed){
        if (!hasAuthority && !isServer){
            CreateProjectile(angle, false, seed);
        }
    }
    
    public override void OnStartClient(){
        base.OnStartClient();
    }

    private void Awake(){
        firingPoint = transform.GetChild(0);
    }

    protected override void Update(){
        base.Update();
    }

    protected override void ClientFixedUpdate(){
        base.ClientFixedUpdate();
        if (isClientOnly){
            if (activelyWielded && shotInSolvo >= 1 && shotInSolvo < shotsPerSalvo){
                HandleFiring(shootingAngle);

            }

            framesLeftTillNextShot--;
            framesLeftTillNextSalvo--;
        }
    }

    protected override void ServerFixedUpdate(){
        base.ServerFixedUpdate();
        if (activelyWielded && shotInSolvo >= 1 && shotInSolvo < shotsPerSalvo){
            HandleFiring(shootingAngle);
                
        }
        framesLeftTillNextShot--;
        framesLeftTillNextSalvo--;
    }

    protected virtual int GetSeed(){
        return 0;
    }
}
