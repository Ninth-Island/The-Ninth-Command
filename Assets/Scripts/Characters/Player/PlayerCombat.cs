using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public partial class Player : Character{

    [Header("Combat")]
    [SerializeField] private int timeTillShieldRecharge;
    [SerializeField] private int shieldRechargeRate;
    private bool _dead;
    
    
    private int _timeLeftTillShieldRecharge;
    private bool _stoppedAudio;


    private void ServerPlayerCombatFixedUpdate(){
        _timeLeftTillShieldRecharge--;
        if (_timeLeftTillShieldRecharge <= 0){
            shield = Mathf.Clamp(shield + shieldRechargeRate, 0, maxShield);
            UpdateHealthClientRpc(health, shield, shield < maxShield, false);
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
    public override void Hit(Player killer, int damage, Vector3 position, float angle){
        if (damage == 0)return;
        
        bool shieldBreak = false;
        if (damage < 0){
            shield -= damage;
        }
        else{
            if (shield > 0){
                shield -= damage;
                if (shield <= 0){
                    health += shield;
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
                    killer.virtualPlayer.kills++;
                    killer.virtualPlayer.score += 100;
                    ServerDie();
                    SetAnimatedBoolOnClientRpc(_aNames.dying, true);
                }
            }
            _timeLeftTillShieldRecharge = timeTillShieldRecharge;
        }
        
        shield = Mathf.Clamp(shield, 0, maxShield);
        health = Mathf.Clamp(health, 0, maxHealth);
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
            audioManager.PlaySound(20);
        }
        else{
            Destroy(Instantiate(armorDamageSparks, position, Quaternion.Euler(0, 0, angle + 180)), 3f);
            audioManager.PlaySound(21);

        }
    }

    [ClientRpc]
    public void UpdateHealthClientRpc(int newHealth, int newShield, bool shieldRegening, bool shieldBreak){

        health = newHealth;
        shield = newShield;
        virtualPlayer.health = health;
        virtualPlayer.shield = shield;
        shieldSlider.value = (float) shield / maxShield;
        healthSlider.value = (float) health / maxHealth;
        
        if (shield > 0){
            healthText.text = "";
            shieldText.text = $"{shield}/{maxShield}";
        }
        else{
            healthText.text = $"{health}/{maxHealth}";
            shieldText.text = "";
        }
        if (hasAuthority){
            ManageHealthShieldSfx(shieldRegening);
        }

        if (shieldBreak){
            audioManager.PlaySound(26);
        }
    }

    [Client]
    private void ManageHealthShieldSfx(bool shieldRegening){
        
        if (!shieldRegening){
            // so for some reason this was null for one day and caused the client to crash on deaths, but it stopped happening so *Shrugs*
            //Debug.Log(audioManager);
            try{
                audioManager.isPlayingCharging = false;

            }
            catch{
                
            }
            if (shield < maxShield / 3){ // warning beeping
                audioManager.PlaySound(23);
                _stoppedAudio = false;
            }
            else{ // not warning, not beeping, not pounding, not regening.
                if (!_stoppedAudio){
                    _stoppedAudio = true;
                    audioManager.source.Stop();
                }
            }

            if (shield <= 0){ 
                _stoppedAudio = false;
                if (health < maxHealth / 3 && health > 0){ // heart pounding
                    audioManager.PlaySound(25);

                }
                else{// really warning beeping
                    audioManager.PlaySound(24);
                }
            }

        }
        else{ // shield regen
            audioManager.PlayChargingNoise(22, (float)shield / maxShield);
            _stoppedAudio = false;
        }
    }

    [Server]
    private void ServerDie(){
        primaryWeapon.StopReloading();
        primaryWeapon.netIdentity.RemoveClientAuthority();
        secondaryWeapon.netIdentity.RemoveClientAuthority();
        Die();
        virtualPlayer.deaths++;
        virtualPlayer.score -= 100;
        _modeManager.Die(this);
        DieClientRpc();


    }

    [ClientRpc]
    private void DieClientRpc(){
        Die();
        audioManager.source.Stop();
    }

    //both
    private void Die(){
        _dead = true;
        
        primaryWeapon.StopReloading();
        primaryWeapon.Drop();
        
        secondaryWeapon.StopReloading();
        secondaryWeapon.Drop();

        armorAbility.StopAllCoroutines();
        armorAbility.Drop();
        gameObject.layer = LayerMask.NameToLayer("Dead Player");
    }

    [Command]
    private void CmdDie(){
        ServerDie();
    }
    
}

