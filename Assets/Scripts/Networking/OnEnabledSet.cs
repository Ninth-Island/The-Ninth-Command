using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class OnEnabledSet : MonoBehaviour{
    
    [SerializeField] private TMP_InputField field;
    [SerializeField] private bool isPort;
    [SerializeField] private bool isHost;
    [SerializeField] private UI ui;
    private void OnEnable(){
        if (isPort){
            field.text = PlayerPrefs.GetString("Port");
            Debug.Log(PlayerPrefs.GetString("Port"));
            if (isHost){
                ui.SetPortHost(field.text);
            }
            else{
                ui.SetPortClient(field.text);
            }
        }
        else{
            field.text = PlayerPrefs.GetString("IP");
            ui.SetTargetIP(field.text);
        }
    }
}
