using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpartanLaser : Projectile{
   
    
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
        Destroy(gameObject);
        /*
        Vector2 pos = transform.position;
        transform.position = new Vector3(pos.x + Mathf.Cos(transform.rotation.z), pos.y + Mathf.Sin(transform.rotation.z));
        GetBody().velocity = new Vector2(0, 0);*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSpartanLaser(ChargingWeapon setSpartanLaser){
    }
}
