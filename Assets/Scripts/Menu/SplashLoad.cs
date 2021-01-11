using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashLoad : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine("LoadGame");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(5);
        AsyncOperation operation = SceneManager.LoadSceneAsync("Tutorial", LoadSceneMode.Single);
    }
}
