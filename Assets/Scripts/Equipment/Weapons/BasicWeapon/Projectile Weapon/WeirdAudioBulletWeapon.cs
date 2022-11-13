using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeirdAudioBulletWeapon : BulletWeapon
{
    protected override void HandleMagazineDecrement(){
        bulletsLeft--;
        audioManager.PlayRepeating(0);
    }
}
