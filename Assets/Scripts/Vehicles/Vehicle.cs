using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : CustomObject{

    [SerializeField] private float embarkRange;
    [SerializeField] private float speed;

    private Player _player;
    private CursorControl _cursorControl;

    private SpriteRenderer driverVisor;
    private SpriteRenderer driver;

    private Player _driverReference;

    private bool _hasDriver;
    
    public float GetEmbarkRange(){
        return embarkRange;
    }

    public override void OnStartClient(){
        base.OnStartClient();
        
        _player = FindObjectOfType<Player>();
        _cursorControl = FindObjectOfType<CursorControl>();

        driverVisor = transform.GetChild(3).GetComponent<SpriteRenderer>();
        driver = transform.GetChild(4).GetComponent<SpriteRenderer>();
        
    }

    protected override void Update(){
        base.Update();
        
        if (Input.GetKeyDown(KeyCode.G) && _hasDriver){
            _hasDriver = false;
            _driverReference.transform.position = transform.position + new Vector3(0, 10);
            _driverReference.gameObject.SetActive(true);

            driver.enabled = false;
            driverVisor.enabled = false;
        }
    }


    public void SetDriver(Player player){
        _driverReference = player;
        _hasDriver = true;
    }
    
    public SpriteRenderer GetDriver(){
        return driver;
    }
    public SpriteRenderer GetDriverVisor(){
        return driverVisor;
    }
    

}