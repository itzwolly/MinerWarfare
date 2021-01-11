using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour
{
    [SerializeField] Vector3 _minPos;
    [SerializeField] Vector3 _maxPos;
    [SerializeField] float _minTime;
    [SerializeField] float _maxTime;
    [SerializeField] float _flightSpeed;
    [SerializeField] GameObject _beckyBrush;

    GameObject _crate;
    GameObject _becky;
    bool _crateExists;
    bool crateExists;
    // Use this for initialization

    public bool HasBecky
    {
        get
        {
            return _crateExists;
        }
    }

    void Start()
    {
        if (!GetComponent<PhotonView>().isMine)
        {
            StartCoroutine("UpdateData");
        }
    }

    private void CallNetworkSpawn()
    {
        Destroy(_crate);
        GetComponent<PhotonView>().RPC("NetworkSpawn", PhotonTargets.All);
    }
    IEnumerator UpdateData()
    {
        while (true)
        {
            _crateExists = crateExists;
            yield return null;
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Debug.Log("Inside OnPhotonSerializeView");
        if (stream.isWriting)
        {
            stream.SendNext(_crate != null);
        }
        else
        {
            crateExists = (bool)stream.ReceiveNext();
        }
    }

    [PunRPC]
    private void NetworkSpawn()
    {
        if (transform.childCount < 1 && GetComponent<PhotonView>().isMine)
        {
            Debug.Log("Network Spanw");
            Vector3 start = new Vector3();
            Vector3 end = new Vector3();
            bool horiz = Random.value < 0.5f;
            bool min = Random.value < 0.5f;

            start.y = _minPos.y;

            if (horiz)
            {
                if (min)
                {
                    start.x = _minPos.x;
                    end.x = _maxPos.x;
                }
                else
                {
                    start.x = _maxPos.x;
                    end.x = _minPos.x;
                }

                start.z = Random.Range(_minPos.z, _maxPos.z);
                end.z = Random.Range(_minPos.z, _maxPos.z);
            }
            else
            {
                if (min)
                {
                    start.z = _minPos.z;
                    end.z = _maxPos.z;
                }
                else
                {
                    start.z = _maxPos.z;
                    end.z = _minPos.z;
                }

                start.x = Random.Range(_minPos.x, _maxPos.x);
                end.x = Random.Range(_minPos.x, _maxPos.x);
            }
            Vector3 dir = (end - start).normalized;
            dir.y = 0;
            ;
            //SpawnBecky();


           // Debug.Log("spawning becky");
            _becky = PhotonNetwork.Instantiate("Becky", start + transform.position, transform.rotation, 0);
            //_becky = Instantiate(_beckyBrush);
            //_becky.transform.position = new Vector3(0, 1000, 0);
            _crate = _becky.GetComponent<BeckyNetworking>().Crate;

            _crateExists = true;
            //_becky.transform.position = start + transform.position;
            _becky.GetComponent<Rigidbody>().AddForce(dir * _flightSpeed, ForceMode.Impulse);
            _becky.GetComponent<BeckyNetworking>().UpdateNetworkingDir(dir, start + transform.position + dir * Random.Range(0, (_maxPos - _minPos).magnitude), end);
        }
    }

    [PunRPC]
    private void SpawnBecky()
    {
        //Debug.Log("spawning becky");
        _becky = PhotonNetwork.Instantiate("Becky", new Vector3(0, 1000, 0), transform.rotation, 0);
        //_becky = Instantiate(_beckyBrush);
        //_becky.transform.position = new Vector3(0, 1000, 0);
        _crate = _becky.GetComponent<BeckyNetworking>().Crate;
    }

    public void ResetCrate()
    {
        GetComponent<PhotonView>().RPC("ResetCrateNetwork", PhotonTargets.All);
    }

    [PunRPC]
    private void ResetCrateNetwork()
    {
        if(GetComponent<PhotonView>().isMine)
        _crate = _becky.GetComponent<BeckyNetworking>().Crate;
    } 

    //IEnumerator UpdateSpawn()
    //{
    //    while (true)
    //    {
    //        if (_crate == null)
    //        {
    //            Debug.Log("start spawningg");
    //            if (_crate == null)
    //            {
    //                _crate = new GameObject();
    //                Invoke("CallNetworkSpawn", Random.Range(_minTime, _maxTime));
    //            }
    //        }
    //        yield return null;
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        _crateExists = (_crate != null);
        if (!_crateExists && PhotonNetwork.isMasterClient && transform.childCount<1)
        {
            //Debug.Log(_crateExists + " | " + (_crate != null));
            _crate = new GameObject();
            _crate.name = "tempCrate";
            Invoke("CallNetworkSpawn", Random.Range(_minTime, _maxTime));
        }
    }

}
