using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : WeaponMod{
    [SerializeField] private Hook hookPrefab;
    [SerializeField] private float fireVelocity;
    [SerializeField] private float maximumVelocity;
    
    public int state; // 0 withdrawn, 1 in air, 2 is attached, 3 is retrieving;
    private Hook _hook;
    private Player _player;    
    private int _framesTillClickAgain;

    
    protected override void OverrideInstant(){
        if (_framesTillClickAgain <= 0){
            _framesTillClickAgain = 12;
            if (state == 0){
                float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
                _hook.gameObject.SetActive(true);
                _hook.gameObject.layer = _player.gameObject.layer - 4;
                _hook.transform.position = transform.position;
                _hook.body.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized * fireVelocity;
                state = 1;
            }

            else if (_hook.locked && state is 1 or 3){ // being sent out or retrieving so recall it
                ResetHook();
            }

            else if (state == 1 && !_hook.locked){ // airborne and unlocked, lock
                _hook.locked = true;
            }
            else if (state == 2){ // attached so recall it
                _hook.Disengage();
                state = 3;
            }
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
        _framesTillClickAgain--;
        if (state == 1 && Vector2.Distance(transform.position, _hook.transform.position) > 100){
            _hook.Disengage();
            state = 3;
        }
        
        if (state == 2){ // attached and met with player
            if (_player.body.velocity.magnitude > maximumVelocity){
                _player.body.velocity = _player.body.velocity.normalized * maximumVelocity;
            }
            
            if (Vector2.Distance(_hook.transform.position, transform.position) < 3){
                ResetHook();
                _player.body.velocity = new Vector2(0, 50);
            }
        }

        if (state == 3 && Vector2.Distance(_hook.transform.position, transform.position) < 3){ // reeling in, done reeling
            ResetHook();
        }
    }

    protected override void Start(){
        base.Start();
        _hook = Instantiate(hookPrefab);
        _hook.SetFirer(this);
        _hook.gameObject.SetActive(false);
        _player = WeaponAttachedTo.wielder;
        Collider2D c = _hook.GetComponent<Collider2D>();
    }

}
