using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class TeamOvercharge : ArmorAbility
{
    [Header("Operator - Team Overcharge")] 
    public float distanceOfCharge;
    public int teamChargePerFrame;
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
        
        foreach (TeammateHUDElements element in wielder.virtualPlayer.Team){
            Player player = element.VirtualPlayer.gamePlayer;
            if (Vector2.Distance(player.transform.position, transform.position) < distanceOfCharge){
                player.shield = Mathf.Clamp(player.shield += teamChargePerFrame, 0, player.maxShield);
                if (isServer){
                    player.UpdateHealthClientRpc(player.health, player.shield, true, false);
                }
            }
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
