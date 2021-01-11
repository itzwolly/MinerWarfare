using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockNetworkBehaviour : Photon.MonoBehaviour {
    [SerializeField] bool _blowUp;

    Vector3 position;
    Quaternion rotation = new Quaternion();
    float smoothing = 10f;
    bool blowUp;
    // Use this for initialization
    void Start () {
        _blowUp = false;
        rotation = transform.rotation;
        position = transform.position;
        if (!photonView.isMine && GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected)
        {
            StartCoroutine("UpdateData");
        }
    }
    
    IEnumerator UpdateData()
    {
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smoothing);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smoothing);
            _blowUp = blowUp;
            yield return null;
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(_blowUp);
        }
        else
        {
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
            blowUp = (bool)stream.ReceiveNext();
        }
    }
}
