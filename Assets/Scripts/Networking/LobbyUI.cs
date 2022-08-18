using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using HSVPicker;
using kcp2k;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour{
    [SerializeField] private TMP_InputField username;
    [SerializeField] private ColorPicker[] pickers;
    [SerializeField] private Image[] playerPreview;
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
        
        if (PlayerPrefs.HasKey("username")){
            _player.GetLobbyPlayer().CmdSetUsername(PlayerPrefs.GetString("username"));
            username.text = PlayerPrefs.GetString("username");
        }

        if (PlayerPrefs.HasKey("visor color")){
            if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString("visor color"), out Color color)){
                _player.GetLobbyPlayer().CmdSetColor(0, color);
                playerPreview[0].color = color;
                pickers[0].CurrentColor = color;
            }
            else{
                PlayerPrefs.DeleteKey("visor color");
            }
        }
    
        if (PlayerPrefs.HasKey("helmet color")){
            if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString("helmet color"), out Color color)){
                _player.GetLobbyPlayer().CmdSetColor(1, color);
                playerPreview[1].color = color;
                pickers[1].CurrentColor = color;
            }
            else{
                PlayerPrefs.DeleteKey("helmet color");
            }
        }
        if (PlayerPrefs.HasKey("arms color")){
            if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString("arms color"), out Color color)){
                _player.GetLobbyPlayer().CmdSetColor(2, color);
                playerPreview[2].color = color;
                pickers[2].CurrentColor = color;
            }
            else{
                PlayerPrefs.DeleteKey("arms color");
            }
        }
        if (PlayerPrefs.HasKey("primary color")){
            if (ColorUtility.TryParseHtmlString(PlayerPrefs.GetString("primary color"), out Color color)){
                _player.GetLobbyPlayer().CmdSetColor(3, color);
                playerPreview[3].color = color;
                pickers[3].CurrentColor = color;
            }
            else{
                PlayerPrefs.DeleteKey("primary color");
            }
        }
        
    }



   

    private bool _isReady;

    public void SetPlayerTeamPosition(TMP_Text text){
        _player.GetLobbyPlayer().CmdSetTeamIndex(int.Parse(text.name));
        text.text = " Click to Join...";
    }

    public void SetUsername(TMP_InputField enteredName){
        _player.GetLobbyPlayer().CmdSetUsername(enteredName.text);
        PlayerPrefs.SetString("username", enteredName.text);
    }

    public void SetColor0(Color color){
        _player.GetLobbyPlayer().CmdSetColor(0, color);
        PlayerPrefs.SetString("visor color", "#" + ColorUtility.ToHtmlStringRGBA(color));
    }
    public void SetColor1(Color color){
        _player.GetLobbyPlayer().CmdSetColor(1, color);
        PlayerPrefs.SetString("helmet color", "#" + ColorUtility.ToHtmlStringRGBA(color));
    }
    public void SetColor2(Color color){
        _player.GetLobbyPlayer().CmdSetColor(2, color);
        PlayerPrefs.SetString("arms color", "#" + ColorUtility.ToHtmlStringRGBA(color));
    }
    public void SetColor3(Color color){
        _player.GetLobbyPlayer().CmdSetColor(3, color);
        PlayerPrefs.SetString("primary color", "#" + ColorUtility.ToHtmlStringRGBA(color));
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
