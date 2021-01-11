using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenScript : MonoBehaviour {

    [SerializeField] GameObject _brokenVersion;
    [SerializeField] float _pieceExplosionMultiplyer;
    [SerializeField] float _pieceStayTime;
    [SerializeField] GameObject _brokenBrush;

    [FMODUnity.EventRef]
    public string _rockBreak;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    virtual public void Break(Vector3 pos, float explosionForce)
    {
        //rock breaking sound
        FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(_rockBreak);
        e.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        e.start();
        e.release();

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
        GameObject broken;
        
        if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
        {
            broken = PhotonNetwork.Instantiate("Rock_02_Shattered", transform.position, transform.rotation, 0);
        }
        else
        {
            broken = Instantiate(_brokenBrush, transform.position, transform.rotation);
        }

        PhotonView view = GetComponent<PhotonView>();

        foreach (Transform t in broken.transform)
        {
            t.gameObject.AddComponent<BrokenPieceScript>();
            t.gameObject.GetComponent<BrokenPieceScript>().SetTime(_pieceStayTime);
            t.gameObject.AddComponent<Rigidbody>();

            //t.SetParent(GameObject.Find("Temp Holder").transform);
            Rigidbody rig = gameObject.GetComponent<Rigidbody>();

            if (rig != null)
            {
                t.gameObject.GetComponent<Rigidbody>().mass = rig.mass /*/ broken.transform.childCount*/;
            }
            else
            {
                t.gameObject.GetComponent<Rigidbody>().mass = 1 /*/ broken.transform.childCount*/;
            }

            t.gameObject.GetComponent<BrokenPieceScript>().Launch(pos, explosionForce, _pieceExplosionMultiplyer);
            t.gameObject.GetComponent<PhotonView>().ObservedComponents[0] = (t.gameObject.GetComponent<BrokenPieceScript>());
        }

        broken.GetComponent<ShatteredBaseNetworking>().DestroyShatteredPiece();

        if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
        {
            view.RPC("networkDestroyBroken", PhotonTargets.All);
        }
        else
        {
            networkDestroyBroken();
        }
    }

    [PunRPC]
    virtual protected void networkDestroyBroken()
    {
        Destroy(gameObject);
    }
}