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
    private TMP_Text[] _playerJoinButtons;

    private Color[] _colors;

    private GameObject _preview;
    private Image[] _previewImages;

    private TMP_Text _name;
    private bool _ready = false;

    public event Action<string, int, Color[]> UpdatePlayerInfo;

    public bool isReady;

    #region Server

    public override void OnStartClient(){
        _username = $"Player {NetworkManager.singleton.numPlayers}";
        _teamIndex = NetworkManager.singleton.numPlayers;

        GameObject[] unsortedList = GameObject.FindGameObjectsWithTag("Team Join Button Text");

        _playerJoinButtons = new TMP_Text[unsortedList.Length];
        foreach (GameObject go in unsortedList){
            _playerJoinButtons[int.Parse(go.name)-1] = go.GetComponent<TMP_Text>();
        }
        

        _colors = new[]{
            new Color(1, 0.5716f, 0, 1),
            new Color(0, 0.7961729f, 1, 1),
            new Color(0, 0.7961729f, 1, 1),
            new Color(0, 0.7961729f, 1, 1)
        };

        _preview = transform.GetChild(1).gameObject;
        _previewImages = new Image[4];
        for (int i = 0; i < 4; i++){
            _previewImages[i] = _preview.transform.GetChild(i).GetComponent<Image>();
            _previewImages[i].color = _colors[i];
        }

        _name = transform.GetChild(0).GetComponent<TMP_Text>();

        foreach (VirtualPlayer virtualPlayer in FindObjectsOfType<VirtualPlayer>()){
            if (virtualPlayer.hasAuthority && virtualPlayer.GetLobbyPlayer() == null){
                virtualPlayer.SetLobbyPlayer(this);
            }
        }

        _ready = true;

    }


    [Command]
    public void CmdSetUsername(string enteredName){
        if (enteredName.Length < 15 && enteredName.Length > 1){
            _username = enteredName;
            _name.text = _username;
        }
    }

    [Command]
    public void CmdSetTeamIndex(int teamIndex){
        if (teamIndex > 0 && teamIndex < 13){
            _teamIndex = teamIndex;
            _name.color = _playerJoinButtons[teamIndex - 1].color;
            if (teamIndex > 6){
                _preview.transform.localScale = new Vector3(-1, 1, 1);
                _preview.transform.localPosition =
                    new Vector3(Mathf.Abs(_preview.transform.localPosition.x) * -1, 0, 0);
            }
            else{
                _preview.transform.localScale = new Vector3(1, 1, 1);
                _preview.transform.localPosition = new Vector3(Mathf.Abs(_preview.transform.localPosition.x), 0, 0);
            }
            
            // if these two lines would work on the client it would be BEAUTIFUL
            // this should work, dunno why it doesnt
            /*transform.SetParent( _playerJoinButtons[teamIndex - 1].transform, false);
            transform.localPosition = Vector3.zero;*/
        }
    }

    /*problems:
     server sees both, works great. Client sees neither
     */


    [Command]
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
        if (_ready){
            transform.SetParent(_playerJoinButtons[newTeamIndex - 1].transform, false);
            transform.localPosition = Vector3.zero;
        }
    }


    #endregion


    public string GetUsername(){
        return _username;
    }
    public Color[] GetColors(){
        return _colors;
    }
}
