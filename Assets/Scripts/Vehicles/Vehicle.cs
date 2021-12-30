using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour{

    [SerializeField] private float embarkRange;
    [SerializeField] private Vector2 offset;

    private Player _player;
    private CursorControl _cursorControl;

    private SpriteRenderer driverVisor;
    private SpriteRenderer driver;
    
    public float GetEmbarkRange(){
        return embarkRange;
    }

    private void Start(){
        _player = FindObjectOfType<Player>();
        _cursorControl = FindObjectOfType<CursorControl>();

        driverVisor = transform.GetChild(3).GetComponent<SpriteRenderer>();
        driver = transform.GetChild(4).GetComponent<SpriteRenderer>();
    }

    private void OnMouseOver(){
        _player.pickupText.SetText("(G) " + name);

        if (Input.GetKeyDown(KeyCode.G)){
            
            _player.SetNotifText("Embarked " + name);
            _player.gameObject.SetActive(false);

            driver.enabled = true;
            driverVisor.enabled = true;

            Transform helmet = _player.transform.GetChild(1).GetChild(5);
            driver.color = helmet.GetChild(0).GetComponent<SpriteRenderer>().color;
            driverVisor.color = helmet.GetChild(2).GetComponent<SpriteRenderer>().color;
            
            
            //AudioManager.PlayFromList(2);
            Embarked(_player);

        }
    }
    private void OnMouseExit(){
        _player.pickupText.SetText("");
    }

    protected virtual void Embarked(Player player){
        
    }
    

}