using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    [SerializeField] private Animator _menuAnimator;
    [SerializeField] private Animator _menuButtonsAnimator;
    [SerializeField] private Animator _creditsAnimator;
    [SerializeField] private Animator _optionsAnimator;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _optionsScreen;
    [SerializeField] private GameObject _creditsScreen;
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private GameObject _logo;
    [SerializeField] private GameObject _exitPopUp;
    [SerializeField] private Image _border;
    [SerializeField] private InputField _inputField;
    [SerializeField] private float _waitTimeTillGameStarts;
    [SerializeField] private Button[] _skinButtons;
    [SerializeField] private Button[] _menuButtons;

    private bool _inMainMenu = true;

    // Use this for initialization
    private void Start() {
        Cursor.lockState = CursorLockMode.None;
        SoundManager.Instance.Initialize("event:/Menu Music");
        LifeTimeManager.Instance.Initialize(_logo, _menuButtonsAnimator, _inputField, _skinButtons);
    }

    private void Update() {
        if (_mainMenu.activeSelf) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (_exitPopUp.activeSelf) {
                    CloseExitPopUp();
                } else {
                    if (_inMainMenu) {
                        SoundManager.Instance.PlayClickSound();
                        _exitPopUp.SetActive(true);
                        setEnabledInteractables(false);
                    }
                }
            }
        }
    }

    private void setEnabledInteractables(bool pEnabled) {
        foreach (Button btn in _skinButtons) {
            ColorBlock btnColors = btn.colors;
            btnColors.disabledColor = btnColors.normalColor;
            btn.colors = btnColors;

            btn.interactable = pEnabled;
        }
        
        foreach (Button btn in _menuButtons) {
            btn.interactable = pEnabled;
        }

        _inputField.interactable = pEnabled;
    }

    public void HandleClick(Button pButton) {
        SoundManager.Instance.PlayClickSound();
        for (int i = 0; i < _skinButtons.Length; i++) {
            ColorBlock btnColors = _skinButtons[i].colors;
            if (_skinButtons[i] == pButton) {
                btnColors.normalColor = new Color(1, 0.64f, 0, 1);
            } else {
                btnColors.normalColor = Color.white;
            }

            _skinButtons[i].colors = btnColors;
        }
    }

    public void SetSkinSelected(string pValue) {
        LifeTimeManager.Instance.SkinName = pValue;
    }

    public void ShowMainMenuIncludingButtons() {
        _inMainMenu = true;
        SoundManager.Instance.PlayClickSound();
        if (_creditsScreen.activeSelf) {
            _creditsAnimator.SetTrigger("SlideUp");
        }
        if (_optionsScreen.activeSelf) {
            _optionsAnimator.SetTrigger("SlideUp");
        }
        _menuAnimator.SetTrigger("Start");
    }

    public void ShowMenuButtons() {
        if (_inputField.text != "" && _inputField.text.Length > 2) {
            _menuButtonsAnimator.SetTrigger("Slide");
            _border.color = Color.clear;
            _logo.SetActive(true);
        } else {
            _logo.SetActive(false);
        }
    }

    public void ShowAudioOptions() {
        if (!_exitPopUp.activeSelf) {
            //SoundManager.Instance.Click(); // This one can be commented out because the sound already occurs in the onvaluechange of the sliders.
            _inMainMenu = false;
            _creditsScreen.SetActive(false);
            _optionsScreen.SetActive(true);
            _menuAnimator.SetTrigger("Audio");
            _optionsAnimator.SetTrigger("SlideDown");
        }
    }

    public void ShowCredits() {
        if (!_exitPopUp.activeSelf) {
            SoundManager.Instance.PlayClickSound();
            _inMainMenu = false;
            _creditsScreen.SetActive(true);
            _optionsScreen.SetActive(false);
            _menuAnimator.SetTrigger("Credits");
            _creditsAnimator.SetTrigger("SlideDown");
        }
    }

    public void ExitGame() {
        SoundManager.Instance.PlayClickSound();
        Application.Quit();
    }

    public void CloseExitPopUp() {
        SoundManager.Instance.PlayClickSound();
        _exitPopUp.SetActive(false);
        setEnabledInteractables(true);
    }

    public void LoadGame() {
        if (!_exitPopUp.activeSelf) {
            _inMainMenu = false;
            SoundManager.Instance.PlayClickSound();

            for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
                PhotonPlayer player = PhotonNetwork.playerList[i];

                if (_inputField.text == player.NickName) {
                    // Taken
                    _border.color = Color.red;
                    _logo.SetActive(false);
                    break;
                } else {
                    if (_inputField.text != "" && _inputField.text.Length > 2) {
                        _border.color = Color.clear;
                        _logo.SetActive(true);
                        StartCoroutine("waitForLoading");
                    } else {
                        _border.color = Color.red;
                        _logo.SetActive(false);
                    }
                }
            }
        }
    }

    public void FireSoundOnValueChanged() {
        if (_optionsScreen.activeSelf) {
            SoundManager.Instance.PlayClickSound();
        }
    }

    private IEnumerator waitForLoading() {
        SetNickName();
        _loadingPanel.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync("InLobby", LoadSceneMode.Single);
        while (!operation.isDone) {
            yield return null;
        }
    }

    public void SetNickName() {
        PhotonNetwork.playerName = _inputField.text;
    }

    public void ValidateName() {
        SoundManager.Instance.PlayClickSound();

        if (_inputField.text == "" || _inputField.text.Length < 3) {
            //_logo.SetActive(false);
        }

        _inputField.text = _inputField.text.Trim();
    }
}
