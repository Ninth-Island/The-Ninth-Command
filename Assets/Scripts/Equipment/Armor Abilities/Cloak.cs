using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Cloak : ArmorAbility
{
    
    [Header("Pathfinder - Cloak")] 
    public float cloakedMoveSpeed;

    public override void ArmorAbilityInstant(float angle){
        if (currentAbilityCharge >= maxCharge){
            active = true;
            StartCoroutine(ResetCloak());
        }
    }

    protected override void AbilityActiveFixedUpdate(){
        float a = 1 - ((float) currentAbilityCharge * 3 / maxCharge);
        SetAlpha(a);
        if (isServer) CloakClientRpc(a);
    }

    private void SetAlpha(float a){
        SetCamoColor(wielder.bodyRenderer, a);
        SetCamoColor(wielder.armRenderer, a);
        SetCamoColor(wielder.helmetRenderer, a);
        SetCamoColor(wielder.visorRenderer, a);
        SetCamoColor(wielder.primaryWeapon.spriteRenderer, a);
    }

    [ClientRpc]
    private void CloakClientRpc(float a){
        SetAlpha(a);
    }
    
    
    private void SetCamoColor(SpriteRenderer sR, float a){
        Color color = sR.color;
        color = new Color(color.r, color.g, color.b, a);
        sR.color = color;
    }
    
    private IEnumerator ResetCloak(){
        float tempSpeed = wielder.moveSpeed;
        wielder.moveSpeed = cloakedMoveSpeed;
        wielder.floatingCanvas.SetActive(false);
        if (isServer){
            PlaySound(0);
        }
        if (hasAuthority){
            AudioManager.PlaySound(0);
        }

        yield return new WaitForSeconds((float) maxCharge / chargeDrainPerFrame / 50);
        SetCamoColor(wielder.bodyRenderer, 1);
        SetCamoColor(wielder.armRenderer, 1);
        SetCamoColor(wielder.helmetRenderer, 1);
        SetCamoColor(wielder.visorRenderer, 1);
        SetCamoColor(wielder.primaryWeapon.spriteRenderer, 1);
        wielder.moveSpeed = tempSpeed;

        wielder.floatingCanvas.SetActive(true);
        ResetCloakClientRpc(tempSpeed);
    }
    
    [ClientRpc]
    private void ResetCloakClientRpc(float tempSpeed){
        SetCamoColor(wielder.bodyRenderer, 1);
        SetCamoColor(wielder.armRenderer, 1);
        SetCamoColor(wielder.helmetRenderer, 1);
        SetCamoColor(wielder.visorRenderer, 1);
        SetCamoColor(wielder.primaryWeapon.spriteRenderer, 1);
        wielder.moveSpeed = tempSpeed;

        wielder.floatingCanvas.SetActive(true);
    }
}
