using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelNetworkingBehaviour : RockScript
{
    [SerializeField] int _maxGems;
    Dictionary<int, string> _dropDictionary;
    GameObject _drop;
    int _numberOfDrop;
    string _typeofDrop;
    [SerializeField] bool _notOnline;

    Vector3 position;
    Quaternion rotation = new Quaternion();
    float smoothing = 10f;

    // Use this for initialization
    void Start ()
    {
        if (!_notOnline)
        {
            transform.SetParent(GameObject.Find("EventController").transform);
            _dropDictionary = new Dictionary<int, string>();
            _dropDictionary.Add(1, "Gem");
            _dropDictionary.Add(2, "PvPPickUp");
            _dropDictionary.Add(3, "BombPickUp");
            _dropDictionary.Add(4, "InstaBomb");

            _numberOfDrop = (int)Random.Range(1, 3);

            if (_numberOfDrop == 1)
            {
                _typeofDrop = _dropDictionary[1];
                _numberOfDrop = Random.Range(1, _maxGems);
            }
            else
            {
                _typeofDrop = _dropDictionary[_numberOfDrop];
                _numberOfDrop = 1;
            }
            if (!GetComponent<PhotonView>().isMine)
            {
                StartCoroutine("UpdateData");
            }
        }
    }

    private void SoloOpenBarel()
    {
        Debug.Log("SoloOpenBarrel");
        GameObject drop = Instantiate(_drop);
        drop.transform.position = transform.position;
        drop.transform.rotation = transform.rotation;
        Destroy(gameObject);
    }

    public void SoloStart(GameObject drop)
    {
        Debug.Log("SoloStart");
        _drop = drop;
        _notOnline = true;
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

    public override void GetDestroyed(Vector3 pos, float explosionForce)
    {
        Debug.Log("This shouldnt happen");
    }


    public override void GetHit(GameObject hitter)
    {
        if (!_notOnline)
        {
            GetComponent<PhotonView>().RPC("OpenBarrel", PhotonTargets.All);
        }
        else
        {
            SoloOpenBarel();
        }
    }

    [PunRPC]
    private void OpenBarrel()
    {
        if (GetComponent<PhotonView>().isMine)
        {
            GameObject drop = PhotonNetwork.Instantiate(_typeofDrop, transform.position, transform.rotation, 0);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Debug.Log("Inside OnPhotonSerializeView");

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
