using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleAnimationIntegration : MonoBehaviour {

    //add animator
    private Animator myAnimator;

    // Use this for initialization
    void Start () {

        myAnimator = GetComponent<Animator>(); //get the animator from the player

    }
	
	// Update is called once per frame
	void Update () {

        //RUN ANIMATION:
        myAnimator.SetFloat("speed", Input.GetAxis("Vertical"));
////////////////////////////////////////////////////////////////////////////////
        //WALKING:

        //if (Input.GetKey(KeyCode.LeftShift))
       // {
            myAnimator.SetFloat("speed", .5f);
        //}
       // else
       // {
            myAnimator.SetFloat("speed", Input.GetAxis("Vertical"));
       // }

       //////////////////////////////////////////////////////////////////////

        //ATTACK ANIMATION:
        //if ()
       // {
            myAnimator.SetBool("kick", true);
       // }
       // else
        //{
            myAnimator.SetBool("kick", false);
        //}

        //////////////////////////////////////////////////////////////////////

        //JUMP ANIMATION:
        //if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
        // {
            myAnimator.SetBool("jump", true);
       // }
       // else
       // {
            myAnimator.SetBool("jump", false);
      //  }

    }
}
