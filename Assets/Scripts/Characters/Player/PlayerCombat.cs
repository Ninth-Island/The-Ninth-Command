using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public partial class Player : Character{


    [SerializeField] private int timeTillShieldRecharge;
    [SerializeField] private int shieldRechargeRate;
    
    private int _timeLeftTillShieldRecharge;


    private void ServerPlayerCombatFixedUpdate(){
        _timeLeftTillShieldRecharge--;
        if (_timeLeftTillShieldRecharge <= 0){
            shield = Mathf.Clamp(shield + shieldRechargeRate, 0, MaxShield);
            UpdateHealthClientRpc(health, shield);
        }
    }
    
    protected override void OnCollisionEnter2D(Collision2D other){
        base.OnCollisionEnter2D(other);
        CheckSoundsOnCollision(other);
        

        if (isServer){
            Projectile projectile = other.gameObject.GetComponent<Projectile>();
            if (projectile){
                Hit(projectile.damage, other.transform.position, projectile.initialAngle); 
                Debug.Log(projectile.initialAngle);
            }
        }
    }
    
    [Server]
    protected override void Hit(int damage, Vector3 position, float angle){
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
                Debug.Log("die");
            }
        }

        _timeLeftTillShieldRecharge = timeTillShieldRecharge;
        ClientSpawnDamageNumberClientRpc(damage, position, angle);
        UpdateHealthClientRpc(health, shield);
    }

    [ClientRpc]
    private void ClientSpawnDamageNumberClientRpc(int damage, Vector3 position, float angle){
        DamageNumber damageNumber = Instantiate(damageTextPrefab, transform.position, Quaternion.identity).GetComponent<DamageNumber>(); 
        damageNumber.damage = damage;
        bool shieldEnough = shield - damage >= 0;
        
        damageNumber.shield = shieldEnough;
        if (shieldEnough){
            Instantiate(shieldDamageSparks, position, Quaternion.Euler(0, 0, angle + 180));
        }
        else{
            Destroy(Instantiate(armorDamageSparks, position, Quaternion.Euler(0, 0, angle + 180)), 3f);
            
        }
    }

    [ClientRpc]
    private void UpdateHealthClientRpc(int newHealth, int newShield){
        health = newHealth;
        shield = newShield;
        shieldSlider.value = (float) shield / MaxShield;
        healthSlider.value = (float) health / MaxHealth;

        
        if (shield > 0){
            healthText.text = "";
            shieldText.text = $"{shield}/{MaxShield}";
        }
        else{
            healthText.text = $"{health}/{MaxHealth}";
            shieldText.text = "";
        }
    }
}

