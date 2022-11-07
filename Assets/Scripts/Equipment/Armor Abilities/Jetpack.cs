using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : ArmorAbility
{
    [Header("Sharpshooter - Jetpack")] 
    public int jetPower;
    public float maximumRise;
    public ParticleSystem jetpack;
    
    private int _jetpackPhase;

    public override void ArmorAbilityReleased(){
        if (_jetpackPhase != 0){
            AudioManager.source.Stop();
            AudioManager.PlaySound(29);
            _jetpackPhase = 0;
            jetpack.Stop();
        }
    }

    public override void ArmorAbilityFixedUpdate(){
        if (wielder._isArmorAbilitying && currentAbilityCharge > 0){
            AbilityActiveFixedUpdate();
        }
        currentAbilityCharge = Mathf.Clamp(currentAbilityCharge + chargeRate, 0, maxCharge);

    }

    protected override void AbilityActiveFixedUpdate(){
        if (_jetpackPhase == 0){
            if (hasAuthority){
                AudioManager.PlayLooping(27);
                StartCoroutine(SetNextJetpackNoise());
                jetpack.Play();
            }

            if (isServer){
                PlaySoundClientRpc(1, 27);
            }
        }

        if (_jetpackPhase == 2){
            if (hasAuthority){
                AudioManager.PlayLooping(28);
            }

            if (isServer){
                PlaySoundClientRpc(2, 28);
            }
        }

        body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y + jetPower, -maximumRise, maximumRise));
        currentAbilityCharge -= chargeDrainPerFrame;
    }


    private IEnumerator SetNextJetpackNoise(){
        _jetpackPhase = 1;
        yield return new WaitForSeconds(4.061f);
        _jetpackPhase = 2;
    }
    
    private void PlaySoundClientRpc(int type, int index){
        if (!hasAuthority){
            if (type == 0){
                AudioManager.PlaySound(index);
            }

            if (type == 1){
                AudioManager.PlayLooping(index);
                StartCoroutine(SetNextJetpackNoise());
                jetpack.Play();
            }

            if (type == 2){
                AudioManager.PlayLooping(28);
            }
        }
    }

    protected override void Start(){
        base.Start();
        jetpack.Stop();
    }
}
