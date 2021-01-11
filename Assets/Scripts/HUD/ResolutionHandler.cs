using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(GameTime))]
public class ResolutionHandler : MonoBehaviour {
    [SerializeField] private GameObject[] _rankEntrys;
    [SerializeField] private GameObject _hud;
    [SerializeField] private Sprite _playerBackground;
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private GameObject _menuButton;

    private RankingsHandler _rankingsHandler;
    private GameTime _gameTime;
    private Transform _parentTransform;
    private GameObject _rankings;

    private bool _finished = false;

    // Use this for initialization
    private void Start() {
        _rankings = GameObject.FindGameObjectWithTag("Rankings");
        GameObject resolution = GameObject.FindGameObjectWithTag("Resolution");
        _parentTransform = resolution.transform;

        _gameTime = GetComponent<GameTime>();
    }

    private void Update() {
        if (_gameTime.HasEnded()) {
            if (!_finished) {
                Debug.Log("The game has ended!");
                DisableScripts();
            }
        }

        if (Input.GetKeyDown(KeyCode.K)) {
            DisableScripts();
        }
    }

    private void DisableScripts() {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player")) {
            StunnedScript stun = obj.GetComponent<StunnedScript>();
            stun.NetworkStun(false);
            MouseOrbitImproved orbit = obj.GetComponentInChildren<MouseOrbitImproved>(true);
            orbit.enabled = false;
        }
        Debug.Log("Disabled scripts");
        EnableMouse();
        ShowResolution();
    }


    private void EnableMouse() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.lockState = CursorLockMode.Confined;
        Debug.Log("Enabled Mouse");
    }

    private void ShowResolution() {
        _hud.SetActive(false);
        _rankings.SetActive(false);
        _menuButton.SetActive(true);

        //play song outro here
        SoundManager.Instance.ChangePart(3.0f);
        _rankingsHandler = _rankings.GetComponent<RankingsHandler>();
        Debug.Log("Loading resolution...");
        Debug.Log("Amount of players in ordered list: " + _rankingsHandler.OrderedList.Count);

        for (int i = 0; i < _rankingsHandler.OrderedList.Count; i++) {
            PlayerRankData data = _rankingsHandler.OrderedList[i];
            GameObject obj = null; //Instantiate(_rankEntry);
            RectTransform rectTransform = null;

            if (data.Rank < 4) {
                obj = Instantiate(_rankEntrys[data.Rank - 1]);
                obj.transform.SetParent(_parentTransform);

                rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.localScale = new Vector3((data.Rank == 1) ? data.Rank : 1f - ((data.Rank - 1f) / 10f), 1, 1);
            } else {
                obj = Instantiate(_rankEntrys[3]);
                obj.transform.SetParent(_parentTransform);

                rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.localScale = new Vector3(0.7f, 1, 1);
            }

            rectTransform.anchoredPosition = new Vector3(0, ((-(rectTransform.rect.height / 2f)) - (rectTransform.rect.height * i)) - 15, 0);

            SetRankingsPlayerData(obj, data.Name, data.Rank, data.Score);
        }

        Debug.Log("Done loading the resolution");
        _finished = true;
    }

    private void SetRankingsPlayerData(GameObject pObj, string pName, int pRank, int pGemsAmount) {
        Text txtRank = pObj.transform.GetChild(0).GetComponent<Text>();
        Text txtName = pObj.transform.GetChild(1).GetComponent<Text>();
        Text txtScore = pObj.transform.GetChild(2).GetComponent<Text>();

        txtRank.text = pRank.ToString();
        txtName.text = pName;
        txtScore.text = pGemsAmount.ToString();

        if (pName == PhotonNetwork.playerName) {
            txtRank.color = Color.white;
            txtName.color = Color.white;
            txtScore.color = Color.white;

            if (pRank > 3) {
                pObj.GetComponent<Image>().sprite = _playerBackground;
            }
        }

        pObj.SetActive(true);
    }

    public void OnMainMenuClick() {
        PhotonNetwork.Disconnect();
        _loadingScreen.SetActive(true);
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu() {
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
        while (!operation.isDone) {
            yield return null;
        }
    }
}
