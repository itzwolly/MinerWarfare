using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MenuMusic : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string _Music;
    //[Header("Values: 0..1")]
    //[SerializeField] float _masterVolume;
    //[SerializeField] float _musicVolume;
    private float a_SongPart;
    private EventInstance _musicStart;

    private VCA _masterVCA;
    private VCA _musicVCA;

    private void Start() {
        _masterVCA = RuntimeManager.GetVCA("vca:/Master");
        _musicVCA = RuntimeManager.GetVCA("vca:/Music");

        _masterVCA.setVolume(PlayerPrefs.GetFloat("MasterVolume", 0.5f));
        _musicVCA.setVolume(PlayerPrefs.GetFloat("MasterVolume", 0.5f));

        _musicStart = FMODUnity.RuntimeManager.CreateInstance(_Music);
        SetParameter(_musicStart, "Health", 100);
        _musicStart.start();
    }

       
    public void ChangePart(float _value)
    {
        SetParameter(_musicStart, "Song Part", _value);
    }

    public void ChangeBattleIntensity(float _value) {
        SetParameter(_musicStart, "Battle", _value);
    }

    void SetParameter(EventInstance e, string name, float value)
    {
        ParameterInstance parameter;
        e.getParameter(name, out parameter);

        parameter.setValue(value);
    }
}