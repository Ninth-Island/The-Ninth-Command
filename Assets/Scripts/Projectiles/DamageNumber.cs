using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DamageNumber : MonoBehaviour{
    public int damage;
    public bool shield;
    [SerializeField] private TextMeshPro text;
    
    private void Start(){
        Destroy(gameObject, 5f);
        text.text = $"{damage}";
        text.fontSize = damage / 12.5f + 10;

        transform.position += new Vector3(Random.Range(-4f, 4f), Random.Range(1f, 5f));
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-30, 30));

        if (shield){
            text.color = new Color(0, 1 - damage / 25f, 1);
        }
        else{
            text.color = new Color(1 - damage / 2000f, 1 - damage / 25, 1 - damage / 25f);
        }

    }

    private void FixedUpdate(){
        text.alpha -= -0.0000326531f * damage + 0.0203265f;
    }
}
