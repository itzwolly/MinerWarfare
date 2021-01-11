using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour {
	public Transform target;
	private float distance = 5.0f;
	private float xSpeed = 120.0f;
	private float ySpeed = 120.0f;
	
	private float yMinLimit = -5f;
	private float yMaxLimit = 80f;
	
	private float distanceMin = .5f;
	private float distanceMax = 15f;
	
	private Rigidbody rigidbody;

    [SerializeField] bool _mouseCentered;
    [SerializeField] PauseToggle _pauseToggle;

    float x = 0.0f;
	float y = 0.0f;

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        rigidbody = GetComponent<Rigidbody>();

        // Make the rigid body not change rotation
        if (rigidbody != null) {
            rigidbody.freezeRotation = true;
        }
    }

    private void Update() {
        if (_pauseToggle.Paused) {
            if (Cursor.lockState != CursorLockMode.None) {
                Cursor.lockState = CursorLockMode.None;
            }
        } else {
            if (Cursor.lockState != CursorLockMode.Locked) {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    void LateUpdate () 
	{
		if (target && Cursor.lockState == CursorLockMode.Locked) 
		{
			x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
			y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			
			y = ClampAngle(y, yMinLimit, yMaxLimit);
			
			Quaternion rotation = Quaternion.Euler(y, x, 0);
			
			distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax);
			
			RaycastHit hit;
			//if (Physics.Linecast (target.position, transform.position, out hit)) 
			//{
			//	distance -=  hit.distance;
			//}
			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = rotation * negDistance + target.position;
			
			transform.rotation = rotation;
			transform.position = position;
		}
	}
	
	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}
}