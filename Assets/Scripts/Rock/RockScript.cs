using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockScript : MonoBehaviour {

    protected System.Random rand = new System.Random();
	// Use this for initialization
	void Start ()
    {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void GetDestroyed(Vector3 pos, float explosionForce)
    {
        BrokenScript script = gameObject.GetComponent<BrokenScript>();
        if (script != null)
        {
            script.Break(pos,explosionForce);
        }
    }
    public virtual void GetHit(GameObject hitter)
    {
        if (hitter.GetComponent<BombBehabiourScript>() != null)
        {
            GetDestroyed(hitter.transform.position, hitter.GetComponent<BombBehabiourScript>().ExplosionForce);
        }
        else
        {
            GetDestroyed(hitter.transform.position, 0);
        }
    }
}
