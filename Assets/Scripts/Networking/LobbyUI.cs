using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using kcp2k;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    
    [SerializeField] private Button[] teamJoinButtons;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;
    [SerializeField] private Sprite[] maps;
    private CustomNetworkManager _networkManager;

    private VirtualPlayer _player;
    private List<LobbyPlayer> _lobbyPlayers;

    private int _mapChoice;


    private void Start(){
        _networkManager = FindObjectOfType<CustomNetworkManager>();
        StartCoroutine(InitializePlayer());
        
    }

    public void ChangeMapChoice(int choice){
        _mapChoice += choice;
    }


    private void SetMap(int map){
        leftArrow.transform.parent.GetChild(0).GetComponent<Image>().sprite = maps[_mapChoice];

    }

    private IEnumerator InitializePlayer(){
        yield return new WaitUntil(()=> NetworkClient.isConnected);
        yield return new WaitUntil(()=> NetworkClient.connection.identity != null);
        _player = NetworkClient.connection.identity.GetComponent<VirtualPlayer>();
        if (!_player.netIdentity.isClientOnly){
            ActivatePanel(leftArrow);
            ActivatePanel(rightArrow); 
        }
    }



   

    private bool _isReady;

    public void SetPlayerTeamPosition(TMP_Text text){
        _player.GetLobbyPlayer().CmdSetTeamIndex(int.Parse(text.name));
        text.text = " Click to Join...";
    }

    public void SetUsername(TMP_InputField enteredName){
        _player.GetLobbyPlayer().CmdSetUsername(enteredName.text);
    }

    public void SetColor0(Color color){
        _player.GetLobbyPlayer().CmdSetColor(0, color);
    }
    public void SetColor1(Color color){
        _player.GetLobbyPlayer().CmdSetColor(1, color);
    }
    public void SetColor2(Color color){
        _player.GetLobbyPlayer().CmdSetColor(2, color);
    }
    public void SetColor3(Color color){
        _player.GetLobbyPlayer().CmdSetColor(3, color);
    }

    public void ReadyUp(Button button){
        if (_player.GetLobbyPlayer().GetIsReady()){
            //if (_player.netIdentity.isClientOnly){
                button.image.color = Color.red;
                button.transform.GetChild(0).GetComponent<TMP_Text>().text = "Not Ready";
                _player.GetLobbyPlayer().CmdSetReady(false);
                /*}
                else{
                    _networkManager.ServerChangeScene(SceneManager.GetSceneByBuildIndex(_mapChoice).name);
                }*/
        }
        else{
            _player.GetLobbyPlayer().CmdSetReady(true);
            button.image.color = Color.green;
            button.transform.GetChild(0).GetComponent<TMP_Text>().text = "Ready";
        }
    }

    private void UpdatePlayerInfo(/*GameObject lobbyPlayer, */string username, int teamIndex, Color[] colors, bool isReady){
        
    }

    public void Disconnect(){
        if (_player.netIdentity.isClientOnly){
            _networkManager.StopClient();
        }
        else{
            _networkManager.StopHost();
        }
        
    }
    

    public void ActivatePanel(GameObject panel){
        panel.SetActive(true);
    }
    
    public void DeactivatePanel(GameObject panel){
        panel.SetActive(false);
    }
    
    public void TogglePanel(GameObject panel){
        panel.SetActive(!panel.activeSelf);
    }
    
    public void LoadScene(int scene){
        SceneManager.LoadScene(scene);
        
    }

    
    public void Quit(){
        Application.Quit();
    }
}
