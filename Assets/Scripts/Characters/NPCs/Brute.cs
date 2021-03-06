using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brute : NPC{
    

    protected override void Start(){
        base.Start();

    }

    protected override void Update(){
        base.Update();
        
        Animator.SetBool(_aNames.Running, false);
        if (Math.Abs(Body.velocity.x) > 0 && !Airborne){
            Animator.SetBool(_aNames.Running, true);
        }
    }

    protected override void FixedUpdate(){
        base.FixedUpdate();
    }

    
    private class ANames{
        public readonly string Running = "Running";
    }

    private ANames _aNames = new ANames();
}
