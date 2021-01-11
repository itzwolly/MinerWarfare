using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;
using FMODUnity;

public class VolumeChanger : MonoBehaviour {
    [SerializeField] private Slider _masterVolume;
    [SerializeField] private Slider _musicVolume;

	// Use this for initialization
	private void Start () {
        _masterVolume.normalizedValue = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        _musicVolume.normalizedValue = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
    }

    public void OnMasterVolumeChange() {
        SoundManager.Instance.SetMasterVolume(_masterVolume.normalizedValue);
    }

    public void OnMusicVolumeChange() {
        SoundManager.Instance.SetMusicVolume(_musicVolume.normalizedValue);
    }
}
