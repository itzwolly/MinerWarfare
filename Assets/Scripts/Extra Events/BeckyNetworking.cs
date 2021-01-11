using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeckyNetworking : Photon.MonoBehaviour {
    [SerializeField] bool _notOnline;
    [SerializeField] Vector3 _distance;
    [SerializeField] GameObject _crate;
    [SerializeField] GameObject _barrelBrush;

    private Vector3 _dropPos;
    private Vector3 _endPos;
    float _dist;

    Vector3 position;
    Quaternion rotation = new Quaternion();
    float smoothing = 10f;

    [PunRPC]
    public void AddBarrel()
    {
        if (!_notOnline && photonView.isMine)
        {
            //Debug.Log("Dropping Barrel");
            //GameObject temp = _crate;
            Destroy(_crate);
            _crate = PhotonNetwork.Instantiate("Item Barrel", transform.position + _distance, transform.rotation, 0);
            GetComponentInParent<EventController>().ResetCrate();
        }
        //PhotonNetwork.Destroy(temp);
    }

    // Use this for initialization
    void Awake() {
        if (!_notOnline)
        {
            transform.SetParent(GameObject.Find("EventController").transform);
            if (!photonView.isMine)
            {
                StartCoroutine("UpdateData");
            }
        }
    }

    public void SoloDrop(float time, GameObject crateBrush ,GameObject dropBrush)
    {
        //Debug.Log("StartingCoroutine");
        StartCoroutine(DropCrateSolo(time, crateBrush,dropBrush));  
    }

    IEnumerator DropCrateSolo(float time, GameObject crateBrush, GameObject dropBrush)
    {
        //Debug.Log("Started Coroutine");
        yield return new WaitForSeconds(time);
        _crate.transform.position = transform.position + _distance;
        _crate.transform.rotation = transform.rotation;
        _crate.GetComponent<BarrelNetworkingBehaviour>().SoloStart(dropBrush);
        Invoke("DestroyThis", 4);
    }

    public GameObject Crate
    {
        get { return _crate;}
    }
	// Update is called once per frame

    public void UpdateNetworkingDir(Vector3 dir, Vector3 drop,Vector3 end)
    {
        photonView.RPC("UpdateDir", PhotonTargets.All,dir,drop,end);
        StartCoroutine("CheckDrop");
    }

    [PunRPC]
    private void UpdateDir(Vector3 dir,Vector3 drop, Vector3 end)
    {
        transform.rotation = Quaternion.LookRotation(dir);
        transform.Rotate(0, 90, 0);
        _dropPos = drop;
        _endPos = end;
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

    IEnumerator CheckDrop()
    {
        while (true)
        {
            _dist = (transform.position - _dropPos).sqrMagnitude;
            //Debug.Log(_dist);
            if (_dist < 2)
            {
                if (_crate != null)
                {
                    photonView.RPC("AddBarrel", PhotonTargets.All);
                    Invoke("DestroyThis", 4);
                }
                break;
            }
            yield return null;
        }
    }
    private void DestroyThis()
    {
        if (_notOnline)
        {
            Destroy(gameObject);
        }
        else
        {
            if (photonView.isMine)
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
