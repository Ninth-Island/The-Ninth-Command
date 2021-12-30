using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warthog : Vehicle
{
    protected override void Embarked(Player player){
        base.Embarked(player);
        Transform sprites = player.transform.GetChild(1);
        for (int i = 0; i < 4; i++){
            sprites.GetChild(i).gameObject.SetActive(false);
        }

    }
}
