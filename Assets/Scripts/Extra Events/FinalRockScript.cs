using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalRockScript : BrokenScript {

    [SerializeField] string _sceneToLoad;

    public override void Break(Vector3 pos, float explosionForce)
    {
        SceneManager.LoadScene(_sceneToLoad);
    }

    protected override void networkDestroyBroken()
    {
        base.networkDestroyBroken();
    }
}
