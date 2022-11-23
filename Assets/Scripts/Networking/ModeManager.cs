using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModeManager : NetworkBehaviour{
    
    [SerializeField] private int gameMode; 
    /* 0 = slayer
     * 1 = capture the flag
     * 2 = capture the zone
     * 3 = siege (destroy enemy base)
     * 4 = deliver the explosive
     */

    [SerializeField] private float timeLeft;

    [SerializeField] private int maxScore;
    private int _bluePoints;
    private Slider _blueSlider;
    private TextMeshProUGUI _blueText;
    
    private int _redPoints;
    private Slider _redSlider;
    private TextMeshProUGUI _redText;

    private TextMeshProUGUI _timerText;

    [SerializeField] private TextMeshProUGUI gameEndText;

    private bool _suddenDeath;
    private bool _changingScene;
    
    private void Start(){

        _blueSlider = transform.GetChild(0).transform.GetChild(0).GetComponent<Slider>();
        _blueText = _blueSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        _redSlider = transform.GetChild(0).transform.GetChild(2).GetComponent<Slider>();
        _redText = _redSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        _timerText = transform.GetChild(0).transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    public void Die(Player player){
        if (gameMode == 0){
            if (player.teamIndex < 7){
                _bluePoints++;
            }
            else{
                _redPoints++;
            }

            if (_redPoints >= maxScore || _bluePoints >= maxScore){
                ServerEndgame();
                
            }
            if (timeLeft <= 0){
                ServerEndgameForReal();
            }
            UpdateScoreClientRpc(_bluePoints, _redPoints);
            player.virtualPlayer.Respawn();
        }
    }
    [Server]
    public void AllPlayersReady(){
        UpdateScoreClientRpc(_bluePoints, _redPoints);
        ServerSetTimeOnClientsRpc(timeLeft);
    }

    [ClientRpc]
    private void UpdateScoreClientRpc(int bluePoints, int redPoints){
        _blueSlider.value = (float) bluePoints / maxScore;
        _blueText.text = $"{bluePoints}/{maxScore}";

        _redSlider.value = (float) redPoints / maxScore;
        _redText.text = $"{redPoints}/{maxScore}";
    }

    private void Update(){
        timeLeft -= Time.deltaTime;
        int minutesInt = Mathf.RoundToInt(timeLeft) / 60;
        int secondsInt = Mathf.RoundToInt(timeLeft) - minutesInt * 60;

        string seconds = secondsInt.ToString();
        if (secondsInt < 10){
            seconds = "0" + seconds;
        }

        if (secondsInt <= 0){
            seconds = "00";
        }
        string minutes = minutesInt.ToString();

        if (minutesInt <= 0){
            minutes = "0";
        }
        _timerText.text = $"{minutes}:{seconds}";
        
        if (timeLeft <= 0 && !_suddenDeath){
            StartCoroutine(EndGame());    
        }
    }


    [Server]
    private void ServerEndgame(){
        EndgameClientRpc();
    }

    [ClientRpc]
    private void EndgameClientRpc(){
        StartCoroutine(EndGame());
    }
    private IEnumerator EndGame(){
        Time.timeScale = 0.2f;
        yield return new WaitForSeconds(0.2f);
        if (_bluePoints == _redPoints){
            _suddenDeath = true;
            yield return new WaitForSeconds(0.4f);
            gameEndText.gameObject.SetActive(true);
            gameEndText.text = "Sudden Death!";
            _timerText.color = Color.red;
            yield return new WaitForSeconds(0.4f);
            Time.timeScale = 1;
            gameEndText.gameObject.SetActive(false);
        }
        else{
            StartCoroutine(EndgameForReal());
        }
    }

    [Server]
    private void ServerEndgameForReal(){
        EndgameForRealClientRpc();
    }

    private void EndgameForRealClientRpc(){
        StartCoroutine(EndgameForReal());
    }


    private IEnumerator EndgameForReal(){
        Time.timeScale = 0.2f;
        gameEndText.gameObject.SetActive(true);
        if (_redPoints > _bluePoints){
            gameEndText.text = "Red Team Wins!";
        }
        else if (_bluePoints > _redPoints){
            gameEndText.text = "Blue Team Wins!";
        }
        else{
            gameEndText.text = "Tie!";
        }

        yield return new WaitForSeconds(1);
        Time.timeScale = 1;
        if (isServer){
            NetworkManager.singleton.StopHost();
        }
        else{
            NetworkManager.singleton.StopClient();
        }
    }

    [ClientRpc]
    private void ServerSetTimeOnClientsRpc(float setTimeLeft){
        timeLeft = setTimeLeft;
    }
}