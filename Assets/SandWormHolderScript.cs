using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandWormHolderScript : Photon.MonoBehaviour {

    [SerializeField] Vector3 _maxMinus;
    [SerializeField] Vector3 _maxPlus;
    [SerializeField] Vector3 _dispacement;
    [SerializeField] GameObject _sandWormBrush;

    [SerializeField] int _wait;
    [SerializeField] int _delay;
    

    [SerializeField] float smoothing = 10;

    GameObject _worm;
    ConnectionStatus _connected;
    void Start()
    {
        _connected = GameObject.Find("Connection Status").GetComponent<ConnectionStatus>();
        //if(_connected)
        //{
        //    if (!photonView.isMine)
        //    {
        //        StartCoroutine("UpdateData");
        //    }
        //}
        Debug.Log(!_connected.Connected || PhotonNetwork.isMasterClient);
        if (!_connected.Connected || PhotonNetwork.isMasterClient)
        {
            Debug.Log("spawningn worm");
            StartSpawnSandWorm();
        }

    }

    private void StartSpawnSandWorm()//put animation for aisdgufadsuygfa in here
    {
        StartCoroutine("SpawnSandWorm", _delay);
        Debug.Log("start spawningn worm " +_delay);
    }



    IEnumerator SpawnSandWorm(int time)
    {
        Debug.Log("spawning worm ------------------!!!!!!!!!!!!!!!!!!!!!!");
        Vector3 pos = new Vector3(Random.Range(transform.position.x + _maxMinus.x, transform.position.x + _maxPlus.x), Random.Range(transform.position.y + _maxMinus.y, transform.position.y + _maxPlus.y), Random.Range(transform.position.z + _maxMinus.z, transform.position.z + _maxPlus.z));

        Debug.Log("Spawning worm at: " + pos);
        pos += _dispacement;
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
        if (_connected.Connected && PhotonNetwork.isMasterClient)
        {
            _worm = PhotonNetwork.Instantiate("SandWorm", pos, rotation, 0);
        }
        else
        {
            Debug.Log("spawned");
            _worm = Instantiate(_sandWormBrush);
            _worm.transform.position = pos;
            _worm.transform.rotation = rotation;
        }
        yield return new WaitForSeconds(time);
        StartCoroutine("StartSpawnSandWorm", _wait);
    }

    IEnumerator UpdateData()
    {
        while (true)
        {
            //transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smoothing);
            //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smoothing);
            yield return null;
        }
    }

    //void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.isWriting)
    //    {
    //        //stream.SendNext(transform.position);
    //        //stream.SendNext(transform.rotation);
    //    }
    //    else
    //    {
    //        //position = (Vector3)stream.ReceiveNext();
    //        //rotation = (Quaternion)stream.ReceiveNext();
    //    }
    //}
}
