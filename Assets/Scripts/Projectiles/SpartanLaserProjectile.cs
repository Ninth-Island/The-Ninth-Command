using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SpartanLaserProjectile : Projectile{
    
    public override void OnStartClient(){
        base.OnStartClient();
    }
    
 
    public override void SetValues(Character firer, int damage, float speed, float angle, bool piercing, int firedLayer, string name){
        base.SetValues(firer, damage, speed, angle, piercing, firedLayer, name);
    }
    
}