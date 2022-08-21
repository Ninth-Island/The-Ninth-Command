using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brute : NPC{
    

    public override void OnStartClient(){
        base.OnStartClient();

    }

    protected override void Update(){
        base.Update();
        
        Animator.SetBool(_aNames.Running, false);
        if (Math.Abs(body.velocity.x) > 0 && !Airborne){
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
