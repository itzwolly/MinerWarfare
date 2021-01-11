using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseBehaviour : MonoBehaviour {
    [SerializeField] private GameObject _optionsScreen;
    [SerializeField] private GameObject _buttonsHolder;
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private NetworkManager _manager;

    private void Start() {
        // Empty
    }

    public void OnResumeClick() {
        // If you get an error here, then it's because you need the network manager to close the pause screen.
        // Reason being is because the network manager is the only way to check for the current player.
        // if we dont do it this way, it might close/open the screen for a different player.
        _manager.CurrentPlayer.GetComponent<PauseToggle>().ClosePauseScreen();
    }

    public void OnOptionsClick() {
        if (gameObject.activeSelf && !_optionsScreen.activeSelf) {
            _buttonsHolder.SetActive(false);
            _optionsScreen.SetActive(true);
        }
    }

    public void OnOptionsBackClick() {
        if (!_buttonsHolder.activeSelf && _optionsScreen.activeSelf) {
            _optionsScreen.SetActive(false);
            _buttonsHolder.SetActive(true);
        }
    }

    public void OnLeaveClick() {
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
