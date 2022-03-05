using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCast : ActiveMod{
    private SpriteRenderer _renderer;
    
    protected override void Start(){
        base.Start();
        
        transform.position = WeaponAttachedTo.firingPoint.transform.position;

        _renderer = GetComponent<SpriteRenderer>();
    }

    protected override void Update(){
        base.Update();
        if (IsReady){
            _renderer.enabled = Input.GetKey(KeyCode.Mouse1);
        }
    }

}
