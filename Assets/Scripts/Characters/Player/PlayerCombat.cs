using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public partial class Player : Character{
    
    [Server]
    public override void Hit(int damage){
        Debug.Log(damage);
        if (shield > 0){
            shield -= damage;
            if (shield < 0){
                health += shield;
            }
        }
        else{
            health -= damage;
            if (health < 0){
                //die
            }
        }
        UpdateHealth(health, shield);
    }

    [ClientRpc]
    private void UpdateHealth(int newHealth, int newShield){
        health = newHealth;
        shield = newShield;
        shieldSlider.value = (float) shield / MaxShield;
        healthSlider.value = (float) health / MaxHealth;
    }
}
