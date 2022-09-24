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
    }

    protected override void FixedUpdate(){
        base.FixedUpdate();
    }

    
    private class ANames{
        public readonly string Running = "Running";
    }

    private ANames _aNames = new ANames();
}
