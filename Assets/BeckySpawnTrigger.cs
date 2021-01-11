using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeckySpawnTrigger : MonoBehaviour
{
    [SerializeField] GameObject _becky;
    [SerializeField] GameObject _crate;
    [SerializeField] GameObject _spawnForCrate;
    [SerializeField] Vector3 _direction;
    [SerializeField] float _timeInDireciton;
    bool _activated;
    // Use this for initialization
    void Start () {
        _activated = false;
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player" && !_activated)
        {
            _activated = true;
            SpawnBecky();
        }
    }

    private void SpawnBecky()
    {
        _becky.transform.position = transform.position;
        _becky.transform.Rotate(_direction);
        _becky.GetComponent<Rigidbody>().AddForce(_direction,ForceMode.Impulse);
        _becky.GetComponent<BeckyNetworking>().SoloDrop(_timeInDireciton, _crate, _spawnForCrate);
    }
    
}
