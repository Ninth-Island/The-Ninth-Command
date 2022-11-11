using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Jetpack : ArmorAbility
{
    [Header("Sharpshooter - Jetpack")] 
    public int jetPower;
    public float maximumRise;
    public ParticleSystem jetpack;

    private bool _finishedStartNoise;

    public override void ArmorAbilityReleased(){
        AudioManager.source.Stop();
        jetpack.Stop();
        AudioManager.PlaySound(2);
        Debug.Log(2);
        _finishedStartNoise = false;

        CmdPlaySoundOnClients(0, 2);
    }

    public override void ArmorAbilityInstant(float angle){
        AudioManager.source.Stop();
        jetpack.Play();
        AudioManager.PlaySound(0);
        _finishedStartNoise = false;
        StartCoroutine(SetNextJetpackNoise());
        CmdPlaySoundOnClients(1, 0);
    }

    public override void ArmorAbilityFixedUpdate(){
        if (wielder._isArmorAbilitying && currentAbilityCharge > 0){
            AbilityActiveFixedUpdate();
        }
        currentAbilityCharge = Mathf.Clamp(currentAbilityCharge + chargeRate, 0, maxCharge);

    }

    protected override void AbilityActiveFixedUpdate(){
        if (_finishedStartNoise){
            AudioManager.PlayLooping(1);
            PlaySoundClientRpc(2, 1);
        }

        wielder.body.velocity = new Vector2(wielder.body.velocity.x, Mathf.Clamp(wielder.body.velocity.y + jetPower, -maximumRise, maximumRise));
        currentAbilityCharge -= chargeDrainPerFrame;
    }


    private IEnumerator SetNextJetpackNoise(){
        yield return new WaitForSeconds(4.061f);
        _finishedStartNoise = true;
    }

    [Command]
    private void CmdPlaySoundOnClients(int type, int index){
        PlaySoundClientRpc(type, index);
    }
    
    [ClientRpc]
    private void PlaySoundClientRpc(int type, int index){
        if (!hasAuthority){
            if (type == 0){
                AudioManager.PlaySound(index);
                AudioManager.source.Stop();
                jetpack.Stop();
                AudioManager.PlaySound(2);
                _finishedStartNoise = false;
            }

            if (type == 1){
                AudioManager.source.Stop();
                jetpack.Play();
                AudioManager.PlaySound(0);
                _finishedStartNoise = false;
                StartCoroutine(SetNextJetpackNoise());
            }

            if (type == 2){
                AudioManager.PlayLooping(index);
            }
        }
    }

    protected override void Start(){
        base.Start();
        jetpack.Stop();
    }
}