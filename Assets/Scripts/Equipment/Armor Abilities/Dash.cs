using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Dash : ArmorAbility
{
    [Header("Assault - Dash")]
    public float dashVelocity;
    public GameObject dashParticles;

    public override void ArmorAbilityInstant(float angle){
        wielder.fallingKnocked = true;
        if (hasAuthority){
            Destroy(Instantiate(dashParticles, wielder.transform.position + new Vector3(-0.37f, 2.05f), Quaternion.Euler(0, 0, angle)), 0.2f);
            AudioManager.PlaySound(0);
        }
        if (isServer){
            CreateDashParticlesClientRpc(angle);
        }
        angle *= Mathf.Deg2Rad;
        wielder.body.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized * (dashVelocity * currentAbilityCharge) / maxCharge;
        currentAbilityCharge = 0;

    }
    
    [ClientRpc]
    private void CreateDashParticlesClientRpc(float angle){
        if (!hasAuthority){
            Destroy(
                Instantiate(dashParticles, transform.position + new Vector3(-0.37f, 2.05f), Quaternion.Euler(0, 0, angle)), 0.2f);
            AudioManager.PlaySound(0);
        }
    }
}

