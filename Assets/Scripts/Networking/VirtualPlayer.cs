using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class VirtualPlayer : NetworkBehaviour{

    private Color[] _colors;

    public string username; // remember to clamp at 15 chars

    public override void OnStartServer(){
        _colors = new Color[4];
        for (int i = 0; i < 4; i++){
            _colors[i] = Color.white;
        }
        
        username = $"Player {NetworkManager.singleton.numPlayers + 1}";
    }

    [Server]
    public void CmdSetUsername(string enteredName){
        if (enteredName.Length < 15 && enteredName.Length > 1){
            username = enteredName;
        }
    }
}
