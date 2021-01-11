using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleIntegration : MonoBehaviour {

    //name of the SFX event
    [FMODUnity.EventRef]
    public string _bombExplode;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        //when you need to trigger the sound
        FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_bombExplode);
        e.start();
        e.release();
    }
}
