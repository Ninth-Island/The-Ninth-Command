using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergizerBeam : Bolt
{
    public override void SetValues(Player setFirer, int setDamage, float speed, float angle, bool piercing, int firedLayer, string setName){
        base.SetValues(setFirer, setDamage, speed, angle, piercing, firedLayer, setName);
        gameObject.layer = LayerMask.NameToLayer("Energizer Beam");
    }
}
