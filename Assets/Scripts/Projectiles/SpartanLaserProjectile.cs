using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpartanLaserProjectile : Projectile{
    
    void Start(){
        Destroy(gameObject, 5f);
    }
    
}
