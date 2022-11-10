using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ShieldOvercharge : ArmorAbility
{
    [Header("Heavy - Shield Overcharge")] 
    public int overChargePerFrame;
    public GameObject fieldPrefab;
    public float fieldScale;
    public Vector2 fieldOffset;

    public override void ArmorAbilityInstant(float angle){
        if (currentAbilityCharge >= maxCharge){
            active = true;
            if (isServer){
                CreateFieldClientRpc(wielder.transform, fieldOffset, fieldScale, (float) maxCharge / chargeDrainPerFrame / 50);
            }

        }
    }

    protected override void AbilityActiveFixedUpdate(){
        wielder.shield = Mathf.Clamp(wielder.shield += overChargePerFrame, 0, wielder.maxShield);
        if (isServer){
            wielder.UpdateHealthClientRpc(wielder.health, wielder.shield, true, false);
        }
    
    }
    
    [ClientRpc]
    private void CreateFieldClientRpc(Transform p, Vector2 offset, float scale, float time){
        GameObject field = Instantiate(fieldPrefab, p);
        field.transform.localPosition = offset;
        field.transform.localScale = new Vector3(scale, scale);
        Destroy(field, time);
        
        AudioManager.PlayNewSource(0, time);
    }
}
