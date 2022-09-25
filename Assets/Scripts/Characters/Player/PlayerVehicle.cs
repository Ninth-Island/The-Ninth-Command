
using Mirror;
using UnityEngine;

public partial class Player : Character{
    
    [Client]
    private void VehicleEmbark(GameObject nearestObject){
        if (Vector2.Distance(transform.position, nearestObject.transform.position) < 70){
            pickupText.SetText("(G) " + nearestObject.name);
            if (Input.GetKeyDown(KeyCode.G)){
                SetNotifText("Embarked " + gameObject.name);
                gameObject.SetActive(false);
                Vehicle vehicle = nearestObject.GetComponent<Vehicle>();
                vehicle.SetDriver(this);
                SpriteRenderer driver = vehicle.GetDriver();
                SpriteRenderer driverVisor = vehicle.GetDriverVisor();
                driver.enabled = true;
                driverVisor.enabled = true;

                Transform helmet = transform.GetChild(1).GetChild(5);
                driver.color = helmet.GetChild(0).GetComponent<SpriteRenderer>().color;
                driverVisor.color = helmet.GetChild(2).GetComponent<SpriteRenderer>().color;
                
                        
            }
        }
    }

}
