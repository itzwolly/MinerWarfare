using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour {
    public GameObject hintSprite;
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)

    {
        if (col.gameObject.tag == "Player")
        {
            hintSprite.SetActive(true);
           
            StartCoroutine(WaitForHint());
        }
    }

    IEnumerator WaitForHint()
    {
        yield return new WaitForSeconds(10);
        //hintSprite.SetActive(!true);
        
    }
}
