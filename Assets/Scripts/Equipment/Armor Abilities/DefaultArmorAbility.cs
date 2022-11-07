using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class DefaultArmorAbility : ArmorAbility
{
    public override void Drop(){
        if (isServer){
            NetworkServer.Destroy(gameObject);
        }
    }
}
