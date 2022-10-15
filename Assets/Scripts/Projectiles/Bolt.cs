using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Bolt : Projectile
{
    /*
* ================================================================================================================
*                                               Bolt
*
*  A plasma bolt that is destroyed instantly upon contact with another surface. Particularly effective at draining shields
* 
* ================================================================================================================
*/

    protected override void OnCollisionEnter2D(Collision2D other){
        base.OnCollisionEnter2D(other);
        Destroy(gameObject, 0.02f);
    }


    public override void SetValues(Character firer, int damage, float speed, float angle, bool piercing, int firedLayer, string name){
        base.SetValues(firer, damage, speed, angle, piercing, firedLayer, name);
    }

}
