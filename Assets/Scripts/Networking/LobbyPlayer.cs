using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : NetworkBehaviour{

    [SyncVar(hook = nameof(ClientHandleChangeDisplayName))]
    private string _username; // remember to clamp at 15 chars

    [SyncVar(hook = nameof(ClientHandleChangeTeamIndex))]
    private int _teamIndex; // 1-6 is team left, 7-12 is team right
    
    [SyncVar(hook = nameof(ClientHandleToggleReady))] 
    private bool _isReady;
    
    [SyncVar(hook = nameof(ClientHandleVisorColor))] private Color _visorColor;
    [SyncVar(hook = nameof(ClientHandleHelmetColor))] private Color _helmetColor;
    [SyncVar(hook = nameof(ClientHandleArmsColor))] private Color _armsColor;
    [SyncVar(hook = nameof(ClientHandlePrimaryColor))] private Color _primaryColor;
    
    private TMP_Text[] _playerJoinButtons;

    private Color[] _colors;

    private GameObject _preview;

    private GameObject _notReadyPreview;
    private Image[] _notReadyPreviewImages;
    
    private GameObject _readyPreview;
    private Image[] _readyPreviewImages;

    private TMP_Text _name;
    private bool _started = false;



    #region Server

    public override void OnStartClient(){
        /*
        _username = $"Player {NetworkManager.singleton.numPlayers}";
        _teamIndex = NetworkManager.singleton.numPlayers;
        */

        GameObject[] unsortedList = GameObject.FindGameObjectsWithTag("Team Join Button Text");

        _playerJoinButtons = new TMP_Text[unsortedList.Length];
        foreach (GameObject go in unsortedList){
            _playerJoinButtons[int.Parse(go.name)-1] = go.GetComponent<TMP_Text>();
        }

        if (_colors == null){
            _colors = new[]{
                new Color(1, 0.5716f, 0, 1),
                new Color(0, 0.7961729f, 1, 1),
                new Color(0, 0.7961729f, 1, 1),
                new Color(0, 0.7961729f, 1, 1)
            };
        }

        _preview = transform.GetChild(1).gameObject;

        _notReadyPreview = _preview.transform.GetChild(0).gameObject;
        _notReadyPreviewImages = new Image[4];

        _readyPreview = _preview.transform.GetChild(1).gameObject;
        _readyPreviewImages = new Image[4];
        
        for (int i = 0; i < 4; i++){
            _notReadyPreviewImages[i] = _notReadyPreview.transform.GetChild(i).GetComponent<Image>();
            _notReadyPreviewImages[i].color = _colors[i];

            _readyPreviewImages[i] = _readyPreview.transform.GetChild(i).GetComponent<Image>();
            _readyPreviewImages[i].color = _colors[i];
        }

        _name = transform.GetChild(0).GetComponent<TMP_Text>();

        foreach (VirtualPlayer virtualPlayer in FindObjectsOfType<VirtualPlayer>()){
            if (virtualPlayer.hasAuthority && virtualPlayer.GetLobbyPlayer() == null){
                virtualPlayer.SetLobbyPlayer(this);
            }
        }

        foreach (LobbyPlayer lobbyPlayer in FindObjectsOfType<LobbyPlayer>()){
            lobbyPlayer.SetupLobbyPlayer();
        }
        _started = true;

    }


    [Command]
    public void CmdSetUsername(string enteredName){
        if (enteredName.Length < 15 && enteredName.Length > 1){
            _username = enteredName;
        }
    }

    [Command]
    public void CmdSetTeamIndex(int teamIndex){
        if (teamIndex > 0 && teamIndex < 13){
            _teamIndex = teamIndex;
        }
    }

    [Command]
    public void CmdSetReady(bool setReady){
        _isReady = setReady;
    }

    [Command]
    public void CmdSetColor(int piece, Color color){
        if (piece >= 0 && piece < 4){
            ClientHandleColorChanged(piece, color);
        }
    }



    #endregion

    #region Client


    [ClientRpc]
    private void ClientHandleColorChanged(int pieceIndex, Color color){
        if (_started){
            _notReadyPreviewImages[pieceIndex].color = color;
            _readyPreviewImages[pieceIndex].color = color;
            _colors[pieceIndex] = color;
        }

        if (pieceIndex == 0){
            ClientHandleVisorColor(Color.clear, color);
        }
        
        else if (pieceIndex == 1){
            ClientHandleHelmetColor(Color.clear, color);
        }
        
        else if (pieceIndex == 2){
            ClientHandleArmsColor(Color.clear, color);
        }
        
        else if (pieceIndex == 3){
            ClientHandlePrimaryColor(Color.clear, color);
        }
    }

    private void ClientHandleChangeDisplayName(string oldName, string newName){
        if (_started){
            _name.text = _username;
        }

    }

    private void ClientHandleChangeTeamIndex(int oldTeamIndex, int newTeamIndex){

        if (_started){
            _name.color = _playerJoinButtons[newTeamIndex - 1].color;
            transform.SetParent(_playerJoinButtons[newTeamIndex - 1].transform, false);
            transform.localPosition = Vector3.zero;
            
            if (newTeamIndex > 6){
                _preview.transform.localScale = new Vector3(-1, 1, 1);
                _preview.transform.localPosition =
                    new Vector3(Mathf.Abs(_preview.transform.localPosition.x) * -1, 0, 0);
            }
            else{
                _preview.transform.localScale = new Vector3(1, 1, 1);
                _preview.transform.localPosition = new Vector3(Mathf.Abs(_preview.transform.localPosition.x), 0, 0);
            }
        }
    }

    private void ClientHandleToggleReady(bool oldReady, bool newReady){
        if (_started){
            _readyPreview.SetActive(newReady);
            _notReadyPreview.SetActive(!newReady);
        }
    }

    private void ClientHandleVisorColor(Color oldColor, Color newColor){
        if (_started){
            _visorColor = newColor;
            _colors[0] = newColor;
            _notReadyPreviewImages[0].color = newColor;
            _readyPreviewImages[0].color = newColor;
        }
    }

    private void ClientHandleHelmetColor(Color oldColor, Color newColor){
        if (_started){
            _helmetColor = newColor;
            _colors[1] = newColor;
            _notReadyPreviewImages[1].color = newColor;
            _readyPreviewImages[1].color = newColor;
        }
    }

    private void ClientHandleArmsColor(Color oldColor, Color newColor){
        if (_started){
            _armsColor = newColor;
            _colors[2] = newColor;
            _notReadyPreviewImages[2].color = newColor;
            _readyPreviewImages[2].color = newColor;
        }
    }

    private void ClientHandlePrimaryColor(Color oldColor, Color newColor){
        if (_started){
            _primaryColor = newColor;
            _colors[3] = newColor;
            _notReadyPreviewImages[3].color = newColor;
            _readyPreviewImages[3].color = newColor;
        }
    }

    [Client]
    private void SetupLobbyPlayer(){
        if (_teamIndex > 0){
            ClientHandleChangeDisplayName("", _username);
            ClientHandleChangeTeamIndex(0, _teamIndex);
            ClientHandleToggleReady(false, _isReady);

            Debug.Log(_colors[0]);
            Debug.Log(_colors[1]);
            Debug.Log(_colors[2]);
            Debug.Log(_colors[3]);
            ClientHandleVisorColor(Color.clear, _colors[0]);
            ClientHandleHelmetColor(Color.clear, _colors[1]);
            ClientHandleArmsColor(Color.clear, _colors[2]);
            ClientHandlePrimaryColor(Color.clear, _colors[3]);
            
        }
        else{
            Debug.Log("Player doesn't have a team index yet");
        }
    }

    #endregion


    public string GetUsername(){
        return _username;
    }
    public Color[] GetColors(){
        return _colors;
    }

    public bool GetIsReady(){
        return _isReady;
    }
}
