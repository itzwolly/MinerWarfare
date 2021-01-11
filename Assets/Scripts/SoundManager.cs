using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager> {
    [EventRef] private string _mainMenuMusic;
    [EventRef] private string _hover;
    [EventRef] private string _click;
    [EventRef] private string _inGameMusic;
    
    private EventInstance _musicInstance;
    private VCA _masterVCA;
    private VCA _musicVCA;
    private bool initialized = false;

    protected SoundManager() { }

    public void Initialize(string pSound) {
        _hover = "event:/Hover";
        _click = "event:/Click";
        _mainMenuMusic = "event:/Menu Music";
        _inGameMusic = "event:/Music";

        _masterVCA = RuntimeManager.GetVCA("vca:/Master");
        _musicVCA = RuntimeManager.GetVCA("vca:/Music");

        _masterVCA.setVolume(PlayerPrefs.GetFloat("MasterVolume", 0.5f));
        _musicVCA.setVolume(PlayerPrefs.GetFloat("MusicVolume", 0.5f));

        StartMusic(pSound);
    }

    public void PlayClickSound() {
        //click sound
        playSound(_click);
    }

    public void PlayHoverSound() {
        //hover sound
        playSound(_hover);
    }

    public void SetMasterVolume(float pNormalizedVolume) {
        if (_masterVCA.isValid()) {
            PlayerPrefs.SetFloat("MasterVolume", pNormalizedVolume);
            PlayerPrefs.Save();
            _masterVCA.setVolume(PlayerPrefs.GetFloat("MasterVolume"));
        }
    }

    public void SetMusicVolume(float pNormalizedVolume) {
        if (_musicVCA.isValid()) {
            PlayerPrefs.SetFloat("MusicVolume", pNormalizedVolume);
            PlayerPrefs.Save();
            _musicVCA.setVolume(PlayerPrefs.GetFloat("MusicVolume"));
        }
    }

    public void ChangePart(float _value) {
        setParameter(_musicInstance, "Song Part", _value);
    }

    public void ChangeBattleIntensity(float _value) {
        setParameter(_musicInstance, "Battle", _value);
    }

    private void playSound(string pPath) {
        EventInstance eventInstance = RuntimeManager.CreateInstance(pPath);

        eventInstance.start();
        eventInstance.release();
    }

    private void setParameter(EventInstance e, string name, float value) {
        ParameterInstance parameter;
        e.getParameter(name, out parameter);

        parameter.setValue(value);
    }

    public void StartMusic(string pSoundString) {
        _musicInstance.stop(STOP_MODE.ALLOWFADEOUT);
        _musicInstance = FMODUnity.RuntimeManager.CreateInstance(pSoundString);
        setParameter(_musicInstance, "Health", 100);
        _musicInstance.start();
    }

    private void OnLevelWasLoaded(int level) {
        Scene activeScene = SceneManager.GetActiveScene();

        switch (activeScene.name) {
            case "Tutorial":
            case "InLobby":
                _musicInstance.stop(STOP_MODE.ALLOWFADEOUT);
                StartMusic(_inGameMusic);
                ChangePart(1.0f);
                break;
            case "MainMenu":
                _musicInstance.stop(STOP_MODE.ALLOWFADEOUT);
                StartMusic(_mainMenuMusic);
                break;
        }
    }
}
