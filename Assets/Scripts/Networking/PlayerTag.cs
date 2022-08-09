using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTag : NetworkBehaviour
{
    [SyncVar(hook = nameof(ClientHandleChangeDisplayName))] private string _username; // remember to clamp at 15 chars
    [SyncVar(hook = nameof(ClientHandleChangeTeamIndex))] private int _teamIndex; // 1-6 is team left, 7-12 is team right

    
    private Color[] _colors;

    private GameObject _preview;
    private Image[] _previewImages;

    private TMP_Text _name;

    public event Action<string, int, Color[]> UpdatePlayerInfo;

    public bool isReady;

    #region Server

    public override void OnStartServer(){
        _username = $"Player {NetworkManager.singleton.numPlayers}";
        _teamIndex = NetworkManager.singleton.numPlayers;
        
        _colors = new[]{
            new Color(1, 0.5716f, 0, 1), 
            new Color(0, 0.7961729f, 1, 1), 
            new Color(0, 0.7961729f, 1, 1),
            new Color(0, 0.7961729f, 1, 1)};

        _preview = transform.GetChild(1).gameObject;
        _previewImages = new Image[4];
        for (int i = 0; i < 4; i++){
            _previewImages[i] = _preview.transform.GetChild(i).GetComponent<Image>();
            _previewImages[i].color = _colors[i];
        }
        
        _name = transform.GetChild(0).GetComponent<TMP_Text>();

        foreach (VirtualPlayer virtualPlayer in FindObjectsOfType<VirtualPlayer>()){
            if (virtualPlayer.hasAuthority){
                virtualPlayer.SetPlayerTag(this);
            }
        }
    }
    

    [Server]
    public void CmdSetUsername(string enteredName){
        if (enteredName.Length < 15 && enteredName.Length > 1){
            _username = enteredName;
            _name.text = _username;
        }
    }

    [Server]
    public void CmdSetTeamIndex(int teamIndex, TMP_Text text){
        if (teamIndex > 0 && teamIndex< 13){
            _teamIndex = teamIndex;
            transform.SetParent(text.transform, false);
            transform.localPosition = Vector3.zero;
            _name.color = text.color;
            if (teamIndex > 6){
                _preview.transform.localScale = new Vector3(-1, 1, 1);
                _preview.transform.localPosition = new Vector3(Mathf.Abs(_preview.transform.localPosition.x) * -1, 0, 0);
            }
            else{
                _preview.transform.localScale = new Vector3(1, 1, 1);
                _preview.transform.localPosition = new Vector3(Mathf.Abs(_preview.transform.localPosition.x), 0, 0);
            }
        }   
    }
    
    [Server]
    public void CmdSetColor(int piece, Color color){
        if (piece >= 0 && piece < 4){
            _colors[piece] = color;
            _previewImages[piece].color = color;
        }
    }
    

    #endregion

    #region Client
    


    public void ClientSetColor(Color color, int pieceIndex){
        _colors[pieceIndex] = color;
        UpdatePlayerInfo?.Invoke(_username, _teamIndex, _colors);
    }
    
    private void ClientHandleChangeDisplayName(string oldName, string newName){
        _username = newName;
        UpdatePlayerInfo?.Invoke(newName, _teamIndex, _colors);
    }

    private void ClientHandleChangeTeamIndex(int oldTeamIndex, int newTeamIndex){
        _teamIndex = newTeamIndex;
        UpdatePlayerInfo?.Invoke(_username, newTeamIndex, _colors);
    }

    #endregion


    public string GetUsername(){
        return _username;
    }
    public Color[] GetColors(){
        return _colors;
    }
}
