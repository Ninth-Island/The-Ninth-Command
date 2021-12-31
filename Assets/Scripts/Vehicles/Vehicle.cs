using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour{

    [SerializeField] private float embarkRange;

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
        
        _player.AddVehicle(new KeyValuePair<GameObject, Vehicle>(gameObject, this));
    }



    public SpriteRenderer GetDriver(){
        return driver;
    }
    public SpriteRenderer GetDriverVisor(){
        return driverVisor;
    }
    

}