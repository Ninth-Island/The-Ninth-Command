using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpartanLaserProjectile : Projectile{
    
    protected override void Start(){
        base.Start();
        Destroy(gameObject, 5f);
    }
    
}