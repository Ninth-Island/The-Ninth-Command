using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class HealingRay : WeaponMod{
    [SerializeField] private int healPerFrame;
    [SerializeField] private Transform endpoint;
    [SerializeField] private LineRenderer[] lines;

    [SerializeField] private int maxFrames;
    [SerializeField] private float amount;
    private bool _toggle;
    private int _framesToSwitch;
    public override void WeaponModInstant(){
        if (Active){
            Active = false;
            OutOfCharge();
        }
        else{
            if (currentAbilityCharge > 0){
                Active = true;
            }
        }
        
        
    }

    private void On(){
        RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right* 3, transform.right, 600,
            LayerMask.GetMask("Team 1", "Team 2", "Team 3", "Team 4", "Ground"));

        if (hit){
            foreach (LineRenderer line in lines){
                line.enabled = true;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, hit.point);
                if (_toggle){
                    line.startWidth += amount;
                }
                else{
                    line.startWidth -= amount;
                }
            }

            if (isServer){
                GameObject player = hit.collider.gameObject;
                if (player.layer != LayerMask.NameToLayer("Ground")){
                    player.GetComponent<Player>().Hit(WeaponAttachedTo.wielder, healPerFrame, hit.point, 0);
                }
            }
        }
        else{
            foreach (LineRenderer line in lines){
                line.enabled = true;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, endpoint.position);
                if (_toggle){
                    line.startWidth += amount;
                }
                else{
                    line.startWidth -= amount;
                }
            }
        }

        _framesToSwitch--;
        if (_framesToSwitch < 0){
            _framesToSwitch = maxFrames;
            _toggle = !_toggle;
        }
    }
    
    protected override void ModActiveFixedUpdate(){
        On();
        if (isServer){
            ServerRunOnClients();
        }
    }

    protected override void OutOfCharge(){
        foreach (LineRenderer line in lines){
            line.enabled = false;
        }
    }

    [Server]
    private void ServerRunOnClients(){
        RunOnClientsRpc();
    }

    [ClientRpc]
    private void RunOnClientsRpc(){
        if (!hasAuthority){
            On();
        }
    }
    protected override void Start(){
        base.Start();

        foreach (LineRenderer line in lines){
            line.useWorldSpace = true;
            line.enabled = false;
        }
    }
}
