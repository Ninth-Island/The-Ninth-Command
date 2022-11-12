using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : WeaponMod{
    [SerializeField] private Hook hookPrefab;
    [SerializeField] private float fireVelocity;
    [SerializeField] private GameObject grappledTo;
    [SerializeField] private float moveToMultiplier;
    [SerializeField] private float maximumVelocity;
    
    public int state; // 0 withdrawn, 1 in air, 2 attached, 3 is retrieving;
    private Hook _hook;
    protected override void OverrideInstant(){
        if (state == 0){
            float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            _hook.gameObject.SetActive(true);
            _hook.gameObject.layer = WeaponAttachedTo.wielder.gameObject.layer - 4;
            _hook.transform.position = transform.position + transform.right * 3;
            _hook.body.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized * fireVelocity;
            state = 1;
        }

        else if (state == 2){
            _hook.Disengage();
            state = 3;
        }

        else if (state is 1 or 3){
            ResetHook();
        }
    }

    private void ResetHook(){
        _hook.gameObject.SetActive(false);
        state = 0;
        _hook.body.bodyType = RigidbodyType2D.Dynamic;
        _hook.ResetHook();

    }

    protected override void FixedUpdate(){
        base.FixedUpdate();
        if (state == 3 && Vector2.Distance(_hook.transform.position, transform.position) < 3){
            ResetHook();
        }

        if (state == 2){
            if (Vector2.Distance(_hook.transform.position, transform.position) < 3){
                ResetHook();
                WeaponAttachedTo.wielder.body.velocity = new Vector2(0, 50);
            }
            if (WeaponAttachedTo.body.velocity.magnitude > maximumVelocity){
                WeaponAttachedTo.body.velocity = WeaponAttachedTo.body.velocity.normalized * maximumVelocity;
            }
        }
    }

    protected override void Start(){
        base.Start();
        _hook = Instantiate(hookPrefab);
        _hook.SetFirer(this);
        _hook.gameObject.SetActive(false);
    }

}
