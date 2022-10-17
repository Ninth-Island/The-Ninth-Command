using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class HostAbilities : MonoBehaviour
{
    private void Update(){
        if (Input.GetKeyDown(KeyCode.P)){
            NetworkManager.singleton.ServerChangeScene("MultiplayerSandbox");
        }
    }
}
