using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseToggle : MonoBehaviour {
    [SerializeField] private MonoBehaviour[] _scriptsToDisable;
    private GameObject _pauseScreen;
    private bool _paused = false;

    public bool Paused {
        get { return _paused; }
        set { _paused = value; }
    }

    public void FillValues(GameObject pPauseScreen) {
        _pauseScreen = pPauseScreen;
    }
	
	// Update is called once per frame
	private void Update () {
        if (_pauseScreen != null) {
            if (Input.GetKeyUp(KeyCode.F1)) {
                if (_paused) {
                    ClosePauseScreen();
                } else {
                    OpenPauseScreen();
                }
            }
        }
    }

    public void ClosePauseScreen() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.lockState = CursorLockMode.Locked;
        _paused = false;
        _pauseScreen.SetActive(false);
        EnableScripts();
    }

    private void OpenPauseScreen() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.lockState = CursorLockMode.Confined;
        _paused = true;
        _pauseScreen.SetActive(true);
        DisableScripts();
    }

    private void EnableScripts() {
        foreach (MonoBehaviour script in _scriptsToDisable) {
            script.enabled = true;
        }
    }

    private void DisableScripts() {
        foreach (MonoBehaviour script in _scriptsToDisable) {
            script.enabled = false;
        }
    }
}
