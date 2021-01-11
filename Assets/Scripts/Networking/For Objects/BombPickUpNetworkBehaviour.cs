using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPickUpNetworkBehaviour : Photon.MonoBehaviour {

    [SerializeField] float smoothing = 10;
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
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "Ground")
        {
            Destroy(GetComponent<Rigidbody>());
            GetComponent<Collider>().isTrigger = true;
            _onGround = true;
        }
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
