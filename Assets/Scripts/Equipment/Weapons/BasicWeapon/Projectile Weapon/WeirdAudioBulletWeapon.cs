using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeirdAudioBulletWeapon : BulletWeapon
{
    protected override void HandleMagazineDecrement(){
        bulletsLeft--;
        AudioManager.PlayRepeating(0);
    }
}
