using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialEnd : MonoBehaviour {

    [SerializeField] string _sceneToLoad;
    [SerializeField] bool _playTutorialAgain;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("PlayedOnce", 0) == 1) {
            SceneManager.LoadScene(_sceneToLoad);
        } else {
            SoundManager.Instance.Initialize("event:/Music");
        }
    }

    // Use this for initialization
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag=="Player")
        {
            SoundManager.Instance.ChangePart(3.0f);
            PlayerPrefs.SetInt("PlayedOnce", 1);
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(_sceneToLoad);
        }
    }

    private void OnValidate()
    {
        if (_playTutorialAgain)
        {
            PlayerPrefs.SetInt("PlayedOnce", 0);
            _playTutorialAgain = false;
        }
    }
}
