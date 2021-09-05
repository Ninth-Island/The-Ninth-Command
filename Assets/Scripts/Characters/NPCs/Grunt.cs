using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grunt : NPC{


    // Start is called before the first frame update
    protected override void Start(){
        base.Start();
        DeathAnimationName = "Dead";
    }


    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

   
    
    protected override void TakeDamage(int damage){
        base.TakeDamage(damage);
    }
}
