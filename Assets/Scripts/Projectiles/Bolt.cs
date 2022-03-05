using System;
using System.Collections;
using System.Collections.Generic;
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
        Destroy(gameObject);
    }

}
