using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpartanLaserProjectile : Projectile{
   
    
    /*
* ================================================================================================================
*                                               Spartan Laser
*
*  Effective at dealing with all types of problems. Not working. Don't use
*
* 
* ================================================================================================================
*/
    
    // Start is called before the first frame update
    void Start(){
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSpartanLaser(ChargingWeapon setSpartanLaser){
    }
}
