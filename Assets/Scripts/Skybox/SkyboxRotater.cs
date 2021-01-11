using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotater : MonoBehaviour {

    public float speedMultiplayer;

    public Material skybox;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        speedMultiplayer = speedMultiplayer + 0.07f;
        skybox.SetFloat("_Rotation", Time.deltaTime + speedMultiplayer);
	}
}
