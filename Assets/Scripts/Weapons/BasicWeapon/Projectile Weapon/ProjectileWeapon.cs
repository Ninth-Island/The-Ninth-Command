using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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

    

    public override void AttemptFire(float angle){
        shootingAngle = angle;
        if (framesLeftTillNextSalvo <= 0){

            if (framesLeftTillNextShot <= 0){

                CreateProjectile(angle);
                HandleMagazineDecrement();
                RefreshText();

                framesLeftTillNextShot = shotDelay;
                shotInSolvo++;
                if (shotInSolvo >= shotsPerSalvo){
                    framesLeftTillNextSalvo = salvoDelay;
                    shotInSolvo = 0;

                }
            }
        }
    }


    protected virtual void CreateProjectile(float angle){
        Projectile projectile = Instantiate(projectileTemplate, firingPoint.position, Quaternion.identity);
        
        // the projectile visually spawns too far forward so to avoid this, the projectile now spawns BEHIND the firing
        // point. The collider is disabled for a frame until it reaches the firing point so it looks better now
        Transform projectileTransform;
        (projectileTransform = projectile.transform).position -= new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)).normalized * projectileSpeed / 50f;
        projectileTransform.position += transform.right * projectileTransform.localScale.x / 4; // dividing by 4 is converting scale units to world units

        Physics2D.IgnoreCollision(projectile.GetCollider(), wielder.Collider); 
        Physics2D.IgnoreCollision(projectile.GetCollider(), wielder.GetFeetCollider());
        projectile.StartCoroutine(projectile.SetValues(projectileDamage, projectileSpeed, angle + Random.Range(-instability, instability), piercing, wielder.gameObject.layer, gameObject.name));
    }

    public override void OnStartClient(){
        base.OnStartClient();
        CursorControl = FindObjectOfType<CursorControl>();
    }

    private void Awake(){
        firingPoint = transform.GetChild(0);
    }

    protected override void Update(){
        base.Update();
    }

    protected override void FixedUpdate(){
        base.FixedUpdate();
        if (activelyWielded && shotInSolvo >= 1 && shotInSolvo < shotsPerSalvo){
            AttemptFire(shootingAngle);
            
        }
        framesLeftTillNextShot--;
        framesLeftTillNextSalvo--;
    }
}
