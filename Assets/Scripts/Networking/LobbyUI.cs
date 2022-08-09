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

        yield return new WaitUntil(() => _player.GetPlayerTag() != null);
        _player.GetPlayerTag().UpdatePlayerInfo += UpdatePlayerInfo;

    }

    private void OnDestroy(){
        if (_player){
            _player.GetPlayerTag().UpdatePlayerInfo -= UpdatePlayerInfo;
        }
    }


   

    private bool _isReady;

    public void SetPlayerTeamPosition(TMP_Text text){
        Debug.Log(_player);
        _player.GetPlayerTag().CmdSetTeamIndex(int.Parse(text.name), text);
        text.text = "  Click to Join...";
    }

    public void SetUsername(TMP_InputField enteredName){
        _player.GetPlayerTag().CmdSetUsername(enteredName.text);
    }

    public void SetColor0(Color color){
        _player.GetPlayerTag().CmdSetColor(0, color);
    }
    public void SetColor1(Color color){
        _player.GetPlayerTag().CmdSetColor(1, color);
    }
    public void SetColor2(Color color){
        _player.GetPlayerTag().CmdSetColor(2, color);
    }
    public void SetColor3(Color color){
        _player.GetPlayerTag().CmdSetColor(3, color);
    }

    public void ReadyUp(Button button){
        if (_player.GetPlayerTag().isReady){
            //if (_player.netIdentity.isClientOnly){
                button.image.color = Color.red;
                button.transform.GetChild(0).GetComponent<TMP_Text>().text = "Not Ready";
                _player.GetPlayerTag().isReady = false;
                /*}
                else{
                    _networkManager.ServerChangeScene(SceneManager.GetSceneByBuildIndex(_mapChoice).name);
                }*/
        }
        else{
            _player.GetPlayerTag().isReady = true;
            button.image.color = Color.green;
            button.transform.GetChild(0).GetComponent<TMP_Text>().text = "Ready";
        }
    }

    private void UpdatePlayerInfo(string username, int teamIndex, Color[] colors){
        teamJoinButtons[teamIndex - 1].transform.GetChild(0).GetComponent<TMP_Text>().text = username;
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
