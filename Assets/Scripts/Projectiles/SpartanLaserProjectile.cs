using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpartanLaserProjectile : Projectile{
    
    protected override void Start(){
        base.Start();
        Destroy(gameObject, 5f);
    }
    
    public override void SetValues(int damage, float speed, float angle, bool piercing, int firedLayer, string name){
        base.SetValues(damage, speed, angle, piercing, firedLayer, name);
    }
    
}