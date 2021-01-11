using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RankingsHandler : MonoBehaviour {
    [SerializeField] private GameObject _opponentRank;
    [SerializeField] private GameObject _playerRank;

    [SerializeField] private Sprite _opponentImage;
    [SerializeField] private Sprite _playerImage;

    private List<PlayerRankData> _playerRankingsHolder = new List<PlayerRankData>();
    private List<GameObject> _rankHolders = new List<GameObject>();
    private List<PlayerRankData> _orderedList = null;

    private Transform _parentObj = null;
    private PlayerRankData _currentPlayerData = null;
    private GameObject _currentGameObject = null;

    public List<PlayerRankData> OrderedList {
        get { return _orderedList; }
    }

    // Use this for initialization
    private void Start() {
        _parentObj = GameObject.FindGameObjectWithTag("Rankings").transform;
    }
    
    public void CreateRankings() {
        for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
            PhotonPlayer pPlayer = PhotonNetwork.playerList[i];
            pPlayer.SetScore(0);
            
            if (_playerRankingsHolder.SingleOrDefault(o => o.Name == pPlayer.NickName) == null) {
                PlayerRankData playerData = new PlayerRankData(pPlayer.NickName, i, pPlayer.GetScore());
                _playerRankingsHolder.Add(playerData);
            }
        }

        if (_currentPlayerData == null) {
            _currentPlayerData = _playerRankingsHolder.SingleOrDefault(o => o.Name == PhotonNetwork.playerName);
        }
        
        UpdatePlayerRankings();
    }

    public void UpdatePlayerRankings() {
        //_orderedList = _playerRankingsHolder.OrderByDescending(o => o.Name != PhotonNetwork.playerName).ThenByDescending(o => o.Score).Take(3).ToList();
        _orderedList = _playerRankingsHolder.OrderByDescending(o => o.Score).ToList();

        if (_rankHolders.Count > 0) {
            for (int i = 0; i < _rankHolders.Count; i++) {
                Destroy(_rankHolders[i]);
            }
            _rankHolders.Clear();
        }

        GameObject fourthObj = null;

        for (int i = 0; i < _orderedList.Count; i++) {
            GameObject obj = Instantiate((_orderedList[i].Name == PhotonNetwork.playerName) ? _playerRank : _opponentRank);
            obj.transform.SetParent(_parentObj);

            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, (-(rectTransform.rect.height / 2f)) - (rectTransform.rect.height * i), 0);
            obj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            _orderedList[i].Rank = i + 1; // update rank

            if (i == 3) {
                fourthObj = obj;
            }

            SetRankingsPlayerData(obj, _orderedList[i].Name, _orderedList[i].Rank, _orderedList[i].Score);
            _rankHolders.Add(obj);
        }

        //if (_currentPlayerData.Rank > 3) {
        //    GameObject currentRankHolder = _rankHolders.SingleOrDefault(o => o.tag == "YourRankPrefab");

        //    RectTransform rectTransform = currentRankHolder.GetComponent<RectTransform>();
        //    currentRankHolder.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, (-(rectTransform.rect.height / 2f)) - (rectTransform.rect.height * 3), 0);
        //    currentRankHolder.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        //    currentRankHolder.SetActive(true);

        //    if (fourthObj != null && fourthObj != currentRankHolder) {
        //        fourthObj.SetActive(false);
        //    }
        //}
    }

    public void UpdateRankingsOnOtherConnected(PhotonPlayer pPlayer) {
        pPlayer.SetScore(0);

        PlayerRankData playerData = new PlayerRankData(pPlayer.NickName, _playerRankingsHolder.Count + 1, pPlayer.GetScore());
        _playerRankingsHolder.Add(playerData);

        UpdatePlayerRankings();
    }

    public void UpdateRankingsOnDisconnect(PhotonPlayer pPlayer) {
        PlayerRankData leavingPlayerData = _playerRankingsHolder.SingleOrDefault(o => o.Name == pPlayer.NickName);
        _playerRankingsHolder.Remove(leavingPlayerData);

        for (int i = 0; i < _playerRankingsHolder.Count; i++) {
            if (_playerRankingsHolder[i].Rank > leavingPlayerData.Rank) {
                _playerRankingsHolder[i].Rank -= 1;
            }
        }

        UpdatePlayerRankings();
    }

    private void SetRankingsPlayerData(GameObject pObj, string pName, int pRank, int pGemsAmount) {
        pObj.transform.GetChild(0).GetComponent<Text>().text = pRank.ToString();
        pObj.transform.GetChild(1).GetComponent<Text>().text = pName;
        pObj.transform.GetChild(2).GetComponent<Text>().text = pGemsAmount.ToString();

        if (pRank < 5) {
            pObj.SetActive(true);
        }
    }

    public void SetScore(int pAmount, PhotonPlayer pPlayer) {
        //Debug.Log(pPlayer.NickName + ": called the setscore");
        pPlayer.SetScore(pAmount);
        PlayerRankData playerData = _playerRankingsHolder.SingleOrDefault(o => o.Name == pPlayer.NickName);

        if (playerData != null) {
            playerData.Score = pPlayer.GetScore();
        }
        
        UpdatePlayerRankings();
    }
}