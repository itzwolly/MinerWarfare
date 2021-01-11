using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour {

	// Use this for initialization
	void Awake ()
    {
        Physics.IgnoreLayerCollision(8, 0);
        Physics.IgnoreLayerCollision(9, 0);
        //Physics.IgnoreLayerCollision(13, 0);
        Physics.IgnoreLayerCollision(13, 9);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
