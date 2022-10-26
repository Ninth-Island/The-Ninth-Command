using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public partial class Player : Character{


    [SerializeField] private int timeTillShieldRecharge;
    [SerializeField] private int shieldRechargeRate;
    private bool _dead;
    
    
    private int _timeLeftTillShieldRecharge;
    private bool _stoppedAudio;

    private void ServerPlayerCombatFixedUpdate(){
        _timeLeftTillShieldRecharge--;
        if (_timeLeftTillShieldRecharge <= 0){
            shield = Mathf.Clamp(shield + shieldRechargeRate, 0, MaxShield);
            UpdateHealthClientRpc(health, shield, shield < MaxShield, false);
        }
    }


    protected override void OnCollisionEnter2D(Collision2D other){
        base.OnCollisionEnter2D(other);
        CheckSoundsOnCollision(other);
        

        if (isServer){
            Projectile projectile = other.gameObject.GetComponent<Projectile>();
            if (projectile){
                Hit(projectile.firer, projectile.damage, other.transform.position, projectile.initialAngle);
            }
        }
    }
    
    [Server]
    protected override void Hit(Player killer, int damage, Vector3 position, float angle){
        bool shieldBreak = false;
        if (shield > 0){
            shield -= damage;
            if (shield <= 0){
                health += shield;
                shield = 0;
                shieldBreak = true;
                if (health <= 0){
                    health = 0;
                    killer.virtualPlayer.kills++;
                    killer.virtualPlayer.score += 100;
                    ServerDie();
                    SetAnimatedBoolOnClientRpc(_aNames.dying, true);
                }
            }
        }
        
        else{
            health -= damage;
            if (health <= 0){
                health = 0;
                killer.virtualPlayer.kills++;
                killer.virtualPlayer.score += 100;
                ServerDie();
                SetAnimatedBoolOnClientRpc(_aNames.dying, true);
            }
        }

        _timeLeftTillShieldRecharge = timeTillShieldRecharge;
        ClientSpawnDamageNumberClientRpc(damage, position, angle);
        UpdateHealthClientRpc(health, shield, false, shieldBreak);
    }

    [ClientRpc]
    private void ClientSpawnDamageNumberClientRpc(int damage, Vector3 position, float angle){
        DamageNumber damageNumber = Instantiate(damageTextPrefab, transform.position, Quaternion.identity).GetComponent<DamageNumber>(); 
        damageNumber.damage = damage;
        bool shieldEnough = shield - damage >= 0;
        
        damageNumber.shield = shieldEnough;
        if (shieldEnough){
            Instantiate(shieldDamageSparks, position, Quaternion.Euler(0, 0, angle + 180));
            AudioManager.PlaySound(20);
        }
        else{
            Destroy(Instantiate(armorDamageSparks, position, Quaternion.Euler(0, 0, angle + 180)), 3f);
            AudioManager.PlaySound(21);

        }
    }

    [ClientRpc]
    private void UpdateHealthClientRpc(int newHealth, int newShield, bool shieldRegening, bool shieldBreak){

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
        if (hasAuthority){
            ManageHealthShieldSfx(shieldRegening);
        }

        if (shieldBreak){
            AudioManager.PlaySound(26);
        }
    }

    [Client]
    private void ManageHealthShieldSfx(bool shieldRegening){
        
        if (!shieldRegening){
            AudioManager.isPlayingCharging = false;
            
            if (shield < MaxShield / 3){ // warning beeping
                AudioManager.PlayLooping(23);
                _stoppedAudio = false;
            }
            else{ // not warning, not beeping, not pounding, not regening.
                if (!_stoppedAudio){
                    _stoppedAudio = true;
                    AudioManager.source.Stop();
                }
            }

            if (shield <= 0){ 
                _stoppedAudio = false;
                if (health < MaxHealth / 3 && health > 0){ // heart pounding
                    AudioManager.PlayLooping(25);

                }
                else{// really warning beeping
                    AudioManager.PlayLooping(24);
                }
            }

        }
        else{ // shield regen
            AudioManager.PlayChargingNoise(22, (float)shield / MaxShield);
            _stoppedAudio = false;
        }
    }

    [Server]
    private void ServerDie(){
        primaryWeapon.netIdentity.RemoveClientAuthority();
        secondaryWeapon.netIdentity.RemoveClientAuthority();
        Die();
        virtualPlayer.deaths++;
        virtualPlayer.score -= 100;
        _level.Die(this);
        DieClientRpc();


    }

    [ClientRpc]
    private void DieClientRpc(){
        Die();
        AudioManager.source.Stop();
    }

    //both
    private void Die(){
        _dead = true;
        
        primaryWeapon.StopReloading();
        primaryWeapon.Drop();
        
        secondaryWeapon.StopReloading();
        secondaryWeapon.Drop();

        gameObject.layer = LayerMask.NameToLayer("Dead Player");
        feetCollider.gameObject.layer = LayerMask.NameToLayer("Dead Player");
    }
    
}

