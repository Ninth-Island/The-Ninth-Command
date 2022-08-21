using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpartanLaserProjectile : Projectile{
    
    public override void OnStartClient(){
        base.OnStartClient();
        Destroy(gameObject, 5f);
    }
    
    public override IEnumerator SetValues(int damage, float speed, float angle, bool piercing, int firedLayer, string name){
        StartCoroutine(base.SetValues(damage, speed, angle, piercing, firedLayer, name));
        yield break;
    }
    
}