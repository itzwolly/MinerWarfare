using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombNetworkBehaviour : Photon.MonoBehaviour {

    [SerializeField] float smoothing = 10;
    Vector3 position;
    Quaternion rotation;
    bool explode;

    // Use this for initialization
    void Start ()
    {
        explode = false;
        if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected && !photonView.isMine)
        {
            StartCoroutine("UpdateData");
        }
        position = transform.position;
        rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update () {
		
	}

    IEnumerator UpdateData()
    {
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smoothing);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smoothing);
            yield return null;
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(explode);
        }
        else
        {
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
            explode = (bool)stream.ReceiveNext();
        }
    }
}
