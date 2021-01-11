using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTime : MonoBehaviour {
    [Header("In Seconds")]
    [SerializeField] private int _goalTime;
    [SerializeField] private Text _timerText;
    [Header("In Seconds")]
    [SerializeField] private float _timeToWaitInLobby;
    [SerializeField] private int _minPlayerReq;

    private string _textTime = "0:00";
    private int _time;
    private bool _startHudTimer = false;
    private float _offset;
    private int _gameTimer;
    private bool _enabledGameTimer = false;

    private PhotonView _photonView;

    // Use this for initialization
    void Start () {
        _time = Convert.ToInt32(_goalTime);
        _offset = Convert.ToSingle(PhotonNetwork.time);

        _photonView = GetComponent<PhotonView>();
    }
	
	// Update is called once per frame
	private void FixedUpdate () {
        if (_startHudTimer) {
            if (_time > 0) {
                _timerText.text = _textTime;
            } else {
                _photonView.RPC("StopGameTimer", PhotonTargets.All);
                _photonView.RPC("StopWaitTimer", PhotonTargets.All);
            }
        }
    }
    
    private IEnumerator HandleGameTimer() {
        while (true) {
            yield return new WaitForSeconds(1);

            _gameTimer++;
            _time = _goalTime - _gameTimer;

            int minutes = _time / 60;
            int seconds = _time % 60;

            _textTime = String.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public IEnumerator HandleStartTimer() {
        while (!_startHudTimer) {
            if (PhotonNetwork.playerList.Length >= _minPlayerReq) {
                yield return new WaitForSeconds(_timeToWaitInLobby);
                _photonView.RPC("StartWaitTimer", PhotonTargets.All);
                _photonView.RPC("StartGameTimer", PhotonTargets.All);
                StopCoroutine(HandleStartTimer());
            } else {
                yield return null;
            }
        }
        StopCoroutine(HandleStartTimer());
    }

    [PunRPC]
    private void StartGameTimer() {
        StartCoroutine(HandleGameTimer());
    }

    [PunRPC]
    private void StopGameTimer() {
        StopCoroutine(HandleGameTimer());
    }

    [PunRPC]
    private void StartWaitTimer() {
        _startHudTimer = true;
    }

    [PunRPC]
    private void StopWaitTimer() {
        _startHudTimer = false;
    }

    public bool HasEnded() {
        if (_time == 0) {
            return true;
        }
        return false;
    }
}
