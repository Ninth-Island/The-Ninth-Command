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

public class UI : MonoBehaviour
{

    // multiplayer
    [SerializeField] private TMP_Text hostError;
    [SerializeField] private TMP_Text clientError;
    private ushort _port = 7777;
    private string _ipAddress = "127.0.0.1";
    private NetworkManager _networkManager;

    private VirtualPlayer _player;


    private void Start(){
        _networkManager = NetworkManager.singleton;
        if (SceneManager.GetActiveScene().name == "Lobby"){
            StartCoroutine(InitializePlayer());
        }
    }

    private IEnumerator InitializePlayer(){
        yield return new WaitUntil(()=> NetworkClient.isConnected);
        yield return new WaitUntil(()=> NetworkClient.connection.identity != null);
        _player = NetworkClient.connection.identity.GetComponent<VirtualPlayer>();
        Debug.Log(_player);
    }


    #region MainMenu

    
    public void SetPortHost(string port){
        bool portAvailable = true;
        if (int.TryParse(port, out int validPort)){
            TcpConnectionInformation[] connectionInformation = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();
            
            foreach (TcpConnectionInformation connection in connectionInformation){
                if (connection.LocalEndPoint.Port == validPort){
                    portAvailable = false;
                    break;
                }
            }
            
            if (portAvailable){
                _port = (ushort) validPort;
                hostError.text = "Valid Port";
                hostError.color = Color.green;

                Debug.Log($"Successfully set port to {_port}");
            }
            else{
                hostError.text = "Port is occupied";
                hostError.color = Color.red;
            }
            
        }
        else{
            hostError.text = "Invalid Port";
            hostError.color = Color.red;
        }
    }
    
    public void SetPortClient(string port){
        bool portAvailable = true;
        if (int.TryParse(port, out int validPort)){
            TcpConnectionInformation[] connectionInformation = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();
            
            foreach (TcpConnectionInformation connection in connectionInformation){
                if (connection.LocalEndPoint.Port == validPort){
                    portAvailable = false;
                    break;
                }
            }
            
            if (portAvailable){
                _port = (ushort) validPort;
                clientError.text = "Valid Port";
                clientError.color = Color.green;

                Debug.Log($"Successfully set port to {_port}");
            }
            else{
                clientError.text = "Port is occupied";
                clientError.color = Color.red;
            }
            
        }
        else{
            clientError.text = "Invalid Port";
            clientError.color = Color.red;
        }
    }

    public void SetTargetIP(string enteredIp){
        bool validateIp = IPAddress.TryParse(enteredIp, out IPAddress validIP);
        if (validateIp){
            _ipAddress = validIP.ToString();
            clientError.text = "Valid Ip";
            clientError.color = Color.green;
            
            Debug.Log($"Successfully set Ip to {_ipAddress}");
        }
        else{
            clientError.text = "Invalid Ip";     
            clientError.color = Color.red;
        }
    }

    public void AttemptHost(){
        _networkManager.GetComponent<KcpTransport>().Port = _port;
        _networkManager.StartHost();
    }
    
    public void AttemptClient(){
        _networkManager.networkAddress = _ipAddress;
        _networkManager.GetComponent<KcpTransport>().Port = _port;
        NetworkManager.singleton.StartClient();
        
    }

    #endregion
    
    
    #region Lobby

    private TMP_Text _overriddenText;

    public void SetPlayerTeamPosition(TMP_Text text){
        if (_overriddenText != null){
            _player.CmdSetTeamIndex(int.Parse(text.name), _overriddenText, text);
        }
        _overriddenText = text;
    }

    public void SetUsername(string enteredName){
        _player.CmdSetUsername(enteredName);
    }

    public void SetColor0(Color color){
        _player.ClientSetColor(color, 0);
    }
    public void SetColor1(Color color){
        _player.ClientSetColor(color, 1);
    }
    public void SetColor2(Color color){
        _player.ClientSetColor(color, 2);
    }
    public void SetColor3(Color color){
        _player.ClientSetColor(color, 3);
    }

    public void Disconnect(){
        if (_player.isClientOnly){
            _networkManager.StopClient();
        }
        else{
            _networkManager.StopHost();
        }
        
    }
    
    #endregion

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
