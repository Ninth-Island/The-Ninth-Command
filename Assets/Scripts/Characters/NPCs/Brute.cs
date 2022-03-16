using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brute : NPC{
    
    private GameObject ArmsController;
    private GameObject UnarmedArms;
    private GameObject WeaponArm1;
    private GameObject WeaponArm2;
    private GameObject OffArm;
    
    protected override void Start(){
        base.Start();

        ArmsController = transform.GetChild(1).GetChild(0).gameObject;
        UnarmedArms = ArmsController.transform.GetChild(0).gameObject;
        WeaponArm1 = ArmsController.transform.GetChild(1).gameObject;
        WeaponArm2 = ArmsController.transform.GetChild(2).gameObject;
        OffArm = ArmsController.transform.GetChild(3).gameObject;

    }

    protected override void Update(){
        base.Update();
    }

    protected override void FixedUpdate(){
        base.FixedUpdate();
    }
    
}
