using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvpToggleNetwork : MonoBehaviour {
    PvpToggle _toggle;
	// Use this for initialization
	void Start () {
        _toggle = GetComponent<PvpToggle>();
	}
	
    [PunRPC]
    public void ToggleOn()
    {

    }
    
}
