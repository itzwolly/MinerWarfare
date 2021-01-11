using UnityEngine;
using System.Collections;

public class RomanControllerScript : MonoBehaviour {

	private Animator myAnimator;
	private Quaternion newrotation;
	private float smooth = 0.05f;
	public Transform camera;

	// Use this for initialization
	void Start () {

		myAnimator = GetComponent<Animator> ();

	
	}
	
	// Update is called once per frame
	void Update () {

		float v = Input.GetAxis ("Vertical");
		float h = Input.GetAxis ("Horizontal");

		movement(v,h);
	
	}
	
	void movement (float v, float h) {

		if (h != 0f || v != 0f) {
			
			//checking if the user pressed any keys
			rotate(v,h);

			
			//RUN ANIMATION:
			myAnimator.SetFloat ("speed", Input.GetAxis ("Vertical"));


			if(Input.GetKey(KeyCode.LeftShift))
			{
				myAnimator.SetFloat ("speed",.5f);
			}
			else {myAnimator.SetFloat ("speed", Input.GetAxis ("Vertical"));
			}


			//ATTACK ANIMATION:
			if (Input.GetKeyDown (KeyCode.E) || Input.GetKeyDown (KeyCode.JoystickButton2)){
				myAnimator.SetBool ("kick", true);
			} else {
				myAnimator.SetBool ("kick", false);
			}


			//JUMP ANIMATION:
			if (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown (KeyCode.JoystickButton0)){
				myAnimator.SetBool ("jump", true);
			} else {
				myAnimator.SetBool ("jump", false);
			}

			//For any additional animation copy for instance the jump animation script above 
			//and replace the input keys to the keys you want to use. Also create a new boolean for your new animation. 
			
			
		}
		else {
			myAnimator.SetFloat ("speed",0);
			//Stop the player if user is not pressing any key
		}
		
	}



	// Rotation Player - You don't need to change anything here

	void rotate(float v,float h) {
		
		if (v > 0)
		{
			if (h > 0)
			{
				newrotation = Quaternion.Euler(0,camera.eulerAngles.y+45,0);
			}
			else if (h < 0)
			{
				newrotation = Quaternion.Euler(0,camera.eulerAngles.y+305,0);
			}
			else
			{
				newrotation = Quaternion.Euler(0,camera.eulerAngles.y,0);
			}
		}
		else if (v < 0)
		{
			if (h > 0)
			{
				newrotation = Quaternion.Euler(0,camera.eulerAngles.y+135,0);
			}
			else if (h < 0)
			{
				newrotation = Quaternion.Euler(0,camera.eulerAngles.y+225,0);
			}
			else {
				newrotation = Quaternion.Euler(0,camera.eulerAngles.y+180,0);
			}
		}
		else
		{
			if (h > 0)
			{
				newrotation = Quaternion.Euler(0,camera.eulerAngles.y+90,0);
			}
			else if (h < 0)
			{
				newrotation = Quaternion.Euler(0,camera.eulerAngles.y+270,0);
			}
			else {
				newrotation = transform.rotation;
			}
		}
		
		newrotation.x = 0;
		newrotation.z = 0;
		//We only want player to rotate in y axis
		transform.rotation = Quaternion.Slerp (transform.rotation,newrotation, smooth);
		//Slerp from player's current rotation to the new intended rotaion smoothly 
		
	}


}
