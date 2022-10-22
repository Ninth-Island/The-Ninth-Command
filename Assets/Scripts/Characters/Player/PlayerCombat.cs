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
            UpdateHealthClientRpc(health, shield, shield < MaxShield);
        }
    }


    protected override void OnCollisionEnter2D(Collision2D other){
        base.OnCollisionEnter2D(other);
        CheckSoundsOnCollision(other);
        

        if (isServer){
            Projectile projectile = other.gameObject.GetComponent<Projectile>();
            if (projectile){
                Hit(projectile.damage, other.transform.position, projectile.initialAngle);
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
            }
        }

        _timeLeftTillShieldRecharge = timeTillShieldRecharge;
        ClientSpawnDamageNumberClientRpc(damage, position, angle);
        UpdateHealthClientRpc(health, shield, false);
    }

    [ClientRpc]
    private void ClientSpawnDamageNumberClientRpc(int damage, Vector3 position, float angle){
        DamageNumber damageNumber = Instantiate(damageTextPrefab, transform.position, Quaternion.identity).GetComponent<DamageNumber>(); 
        damageNumber.damage = damage;
        bool shieldEnough = shield - damage >= 0;
        
        damageNumber.shield = shieldEnough;
        if (shieldEnough){
            Instantiate(shieldDamageSparks, position, Quaternion.Euler(0, 0, angle + 180));
            AudioManager.PlaySound(20, false);
        }
        else{
            Destroy(Instantiate(armorDamageSparks, position, Quaternion.Euler(0, 0, angle + 180)), 3f);
            AudioManager.PlaySound(21, false);

        }
    }

    [ClientRpc]
    private void UpdateHealthClientRpc(int newHealth, int newShield, bool shieldRegening){

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
        
        ManageHealthShieldSfx(shieldRegening);

    }

    [Client]
    private void ManageHealthShieldSfx(bool shieldRegening){
        
        if (!shieldRegening){
            if (shield < MaxShield / 3){ // warning beeping
                PlayLoopedSound(23);
            }
            else{ // not warning, not beeping, not pounding, not regening.
                _secondSource.Stop();
            }
            if (shield <= 0){ 
                if (health < MaxHealth / 3){ // heart pounding
                    PlayLoopedSound(25);
                }
                else{// really warning beeping
                    PlayLoopedSound(24);
                }
            }

        }
        else{ // shield regen
            _secondSource.timeSamples = Mathf.RoundToInt(_secondSource.clip.samples * Mathf.Clamp((float) shield / MaxShield, 0, 1)) -1;
            PlayLoopedSound(22);
        }
    }

    [Client]
    private void PlayLoopedSound(int index){
        if (!_secondSource.isPlaying || _secondSource.clip != AudioManager.sounds[index].clipsList[0]){
            _secondSource.clip = AudioManager.sounds[index].clipsList[0];
            _secondSource.Play();
        }
    }
}

