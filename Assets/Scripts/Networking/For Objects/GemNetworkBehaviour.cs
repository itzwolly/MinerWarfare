using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemNetworkBehaviour : Photon.MonoBehaviour {

    [SerializeField] float smoothing = 10;
    [SerializeField] float seconds = 5;
    Vector3 position;
    Quaternion rotation;
    bool _onGround;
 
    // Use this for initialization
    void Start ()
    {
        position = transform.position;
        rotation = transform.rotation;
        if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected && !photonView.isMine)
        {
            StartCoroutine("UpdateData");
            StartCoroutine("WaitForDelete");
        }

    }

    public void NotOnGround()
    {
        _onGround = false;
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ground")
        {
            Destroy(GetComponent<Rigidbody>());
            GetComponent<Collider>().isTrigger = true;
            _onGround = true;
        }
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

    IEnumerator WaitForDelete()
    {
        yield return new WaitForSeconds(seconds);
        if(photonView.isMine)
        {
            photonView.RPC("DestroyGemInNewtork", PhotonTargets.All);
        }
    }

    void DestroyGemInNewtork()
    {
        Destroy(gameObject);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!_onGround)
        {
            if (stream.isWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
            }
            else
            {
                position = (Vector3)stream.ReceiveNext();
                rotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }
}
