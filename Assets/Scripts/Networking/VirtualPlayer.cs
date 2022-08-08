using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class VirtualPlayer : NetworkBehaviour{

    private string _username; // remember to clamp at 15 chars
    private int _teamIndex; // 1-6 is team left, 7-12 is team right
    [SerializeField]private Color[] _colors;

    
    // client has no connection to server
    
    
    public override void OnStartServer(){
        _colors = new Color[4];
        for (int i = 0; i < 4; i++){
            _colors[i] = Color.white;
        }
        
        _username = $"Player {NetworkManager.singleton.numPlayers}";
    }

    [Server]
    public void CmdSetUsername(string enteredName){
        if (enteredName.Length < 15 && enteredName.Length > 1){
            _username = enteredName;
        }
    }

    [Server]
    public void CmdSetTeamIndex(int teamIndex, TMP_Text oldText, TMP_Text newText){
       Debug.Log(teamIndex);
        if (teamIndex > 0 && teamIndex< 12){
            _teamIndex = teamIndex;
            oldText.text = "  Click to Join...";
            newText.text = _username;
        }
    }

    [Client]
    public void ClientSetColor(Color color, int pieceIndex){
        _colors[pieceIndex] = color;
    }
    


    public string GetUsername(){
        return _username;
    }
}
