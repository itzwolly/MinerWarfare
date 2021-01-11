using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandWormScript : Photon.MonoBehaviour
{
    [SerializeField] float _lifeTime;
    [Range(0,100)]
    [SerializeField] int _gemLossPercentage;
    [SerializeField] float _addedForce;
    [SerializeField] float _explosionForce;

    [SerializeField] float smoothing = 10;
    Vector3 position;
    Quaternion rotation;


    // Use this for initialization
    void Start () {
        Invoke("DestroySandWorm", _lifeTime);
        transform.SetParent(GameObject.Find("SandWormHolder").transform);
        if (GameObject.Find("Connection Status").GetComponent<ConnectionStatus>().Connected && !photonView.isMine)
        {
            StartCoroutine("UpdateData");
        }
        position = transform.position;
        rotation = transform.rotation;
    }

    private void DestroySandWorm()
    {
        PhotonNetwork.Destroy(gameObject);
    }
	
	// Update is called once per frame

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            Debug.Log("Worm hit player ........ ------------------ !!!!!!!!!!!!!!!!");
            //GetComponent<Collider>().enabled = false;
            //Invoke("ReanableCollision", 1);
            GameObject obj = collision.gameObject;
            //if (obj.GetComponent<Rigidbody>() != null)
            {
                //Debug.Log("BOOOOOM SHAKA LAKA");
                Vector3 explosion = (obj.transform.position - transform.position).normalized;
                explosion.y = _addedForce;
                explosion *= _explosionForce;
                obj.GetComponent<Rigidbody>().AddForce(explosion, ForceMode.Impulse);

                // Lose Gems
                GemCollectionScript gemColl = obj.GetComponent<GemCollectionScript>();
                gemColl.LoseGems(_gemLossPercentage);

                StunnedScript stun = obj.GetComponent<StunnedScript>();
                if (stun != null)
                {
                    stun.NetworkStun(true);
                }
            }
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

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
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
